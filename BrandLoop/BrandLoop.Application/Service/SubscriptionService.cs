using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.SubcriptionModel;
using BrandLoop.Shared.Helper;
using IdGen;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaySystem _paySystem;
        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper, IPaymentRepository paymentRepository, IUserRepository userRepository, IPaySystem paySystem)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _paySystem = paySystem;
        }
        public async Task<List<SubscriptionDTO>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsAsync();
            return _mapper.Map<List<SubscriptionDTO>>(subscriptions);
        }
        public async Task<SubscriptionDTO> GetSubscriptionByIdAsync(int subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            return _mapper.Map<SubscriptionDTO>(subscription);
        }
        public async Task<SubscriptionDTO> AddSubscriptionAsync(AddSubcription subscription)
        {
            var subscriptionEntity = new Subscription
            {
                SubscriptionName = subscription.SubscriptionName,
                Duration = subscription.Duration,
                Price = subscription.Price,
                Description = subscription.Description
            };
            var sub = await _subscriptionRepository.AddSubscriptionAsync(subscriptionEntity);
            return _mapper.Map<SubscriptionDTO>(sub);
        }
        public async Task<SubscriptionDTO> UpdateSubscriptionAsync(SubscriptionDTO subscription)
        {

            var updatedSub = await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
            return _mapper.Map<SubscriptionDTO>(updatedSub);
        }
        public async Task DeleteSubscriptionAsync(int subscriptionId)
        {
            await _subscriptionRepository.DeleteSubscriptionAsync(subscriptionId);
        }
        public async Task<List<SubscriptionRegisterDTO>> GetSubscriptionRegistersOfUser(string userId)
        {
            var subscriptionRegisters = await _subscriptionRepository.GetSubscriptionRegistersOfUser(userId);
            return _mapper.Map<List<SubscriptionRegisterDTO>>(subscriptionRegisters);
        }
        public async Task<SubscriptionRegisterDTO> GetSubscriptionRegisterByIdAsync(int id)
        {
            var subscriptionRegisters = await _subscriptionRepository.GetSubscriptionRegisterByIdAsync(id);
            return _mapper.Map<SubscriptionRegisterDTO>(subscriptionRegisters);
        }
        public async Task<PaymentSubscription> RegisterSubscription(string userID, int subscriptionId)
        {
            var now = DateTimeHelper.GetVietnamNow();
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            if (subscription == null)
                throw new Exception($"Subscription with ID {subscriptionId} not found.");
            if (subscription.isDeleted)
                throw new Exception($"Subscription {subscription.SubscriptionName} is not available now.");

            var user = await _userRepository.GetBasicAccountProfileAsync(userID);
            if (user == null)
                throw new Exception($"User with ID {userID} not found.");

            var subscriptionRegister = new SubscriptionRegister
            {
                UID = userID,
                SubscriptionId = subscriptionId,
                Status = RegisterSubStatus.Pending,
                RegistrationDate = now,
                ExpirationDate = now.AddDays((double)subscription.Duration)
            };
            var registeredSub = await _subscriptionRepository.RegisterSubscription(subscriptionRegister);

            // Tạo mã thanh toán
            var orderCode = await GenerateOrderCode();
            
            var payment = new Payment
            {
                PaymentId = orderCode,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                Amount = (int)subscription.Price,
                Status = PaymentStatus.pending,
                Type = PaymentType.subscription,
                SubscriptionRegisterId = registeredSub.Id,
                PaymentMethod = "Bank Transfer",
                TransactionCode = "Not bank yet"
            };
            await _paymentRepository.CreatePaymentAsync(payment);
            return _mapper.Map<PaymentSubscription>(registeredSub);
        }

        public async Task<CreatePaymentResult> CreatePaymentLink(string userID, long orderCode)
        {
            var user = await _userRepository.GetBasicAccountProfileAsync(userID);
            if (user == null)
                throw new Exception($"User with ID {userID} not found.");

            var payment = await _paymentRepository.GetPaymentByIdAsync(orderCode);
            if (payment == null)
                throw new Exception($"Payment with ID {orderCode} not found.");
            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Payment is not in pending status.");

            var subscriptionRegister = await _subscriptionRepository.GetSubscriptionRegisterByIdAsync((int)payment.SubscriptionRegisterId);
            if (subscriptionRegister == null)
                throw new Exception($"Subscription Register with ID {payment.SubscriptionRegisterId} not found.");

            var item = new ItemData($"Subscription {subscriptionRegister.Subscription.SubscriptionName}", payment.Amount, 1);
            List<ItemData> items = new List<ItemData>();
            items.Add(item);

            var paymentInfo = await _paySystem.CreatePaymentAsync(
                user,
                $"Payment for subscription",
                orderCode,
                items
            );
            await _paymentRepository.UpdatePaymentTransactionCode(orderCode, paymentInfo.paymentLinkId);
            return paymentInfo;
        }

        public async Task ConfirmPayment(long orderCode)
        {
            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(orderCode);
            if (payment == null)
                throw new Exception($"Payment with order code {orderCode} not found.");
            if (payment.Status != PaymentStatus.pending)
                throw new Exception("Payment is not in pending status.");

            var paymentLinkInfo = await _paySystem.getPaymentLinkInformation(orderCode);
            if (paymentLinkInfo.status != "PAID")
                throw new Exception($"Payment is not paid yet: {paymentLinkInfo.status}");

            await _paymentRepository.UpdatePaymentStatus(orderCode, PaymentStatus.Succeeded);
            await _subscriptionRepository.UpdateRegisterStatus(payment.SubscriptionRegister.Id, RegisterSubStatus.Active);
        }

        public async Task CancelPayment(long orderCode)
        {
            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(orderCode);
            if (payment == null)
                throw new Exception($"Payment with order code {orderCode} not found.");

            var paymentLinkInfo = await _paySystem.cancelPaymentLink(orderCode, "User cancelled the payment");

            await _paymentRepository.UpdatePaymentStatus(orderCode, PaymentStatus.Canceled);
            await _subscriptionRepository.UpdateRegisterStatus(payment.SubscriptionRegister.Id, RegisterSubStatus.Cancelled);
        }
        public async Task<long> GenerateOrderCode()
        {
            // Time-based + random số nhỏ, đảm bảo trong giới hạn Int64
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 13 chữ số
            var random = new Random().Next(100, 999); // 3 chữ số
            var combined = $"{timestamp}{random}"; // Tổng: 16 chữ số

            var result = long.Parse(combined);
            var checkExistingPayment = await _paymentRepository.GetPaymentByOrderCodeAsync(result);
            if (checkExistingPayment != null)
                // Nếu đã tồn tại, gọi lại hàm để tạo mã mới
                return await GenerateOrderCode();

            return result;
        }

    }
}
