using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AdminDashboardService(IPaymentRepository paymentRepository, IUserRepository userRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaymentChart> GetPaymentChart(int? year)
        {
            var payments = await _paymentRepository.GetAllPaymentsByYear(year);
            var result = new PaymentChart();

            if (payments == null || !payments.Any())
                return result;

            result.totalTransactions = payments.Count;
            result.totalAmount = payments.Sum(p => p.Amount);

            result.totalSubscriptionsPayments = payments.Count(p => p.Type == Domain.Enums.PaymentType.subscription);
            result.totalCampaignsPayments = payments.Count(p => p.Type == Domain.Enums.PaymentType.campaign);

            result.totalSubscriptionMoney = payments.Where(p => p.Type == Domain.Enums.PaymentType.subscription).Sum(p => p.Amount);
            result.totalCampaignsMoney = payments.Where(p => p.Type == Domain.Enums.PaymentType.campaign).Sum(p => p.Amount);

            for (int i = 1; i <= 12; i++)
            {
                var monthlyPayments = payments.Where(p => p.CreatedAt.Month == i).ToList();
                var monthlySubscriptions = monthlyPayments.Where(p => p.Type == Domain.Enums.PaymentType.subscription);
                var monthlyCampaigns = monthlyPayments.Where(p => p.Type == Domain.Enums.PaymentType.campaign);
                result.paymentPerMonths.Add(new PaymentPerMonth
                {
                    month = i,
                    totalSubscriptionsPayments = monthlySubscriptions.Count(),
                    totalSubscriptionMoney = monthlySubscriptions.Sum(p => p.Amount),
                    totalCampaignsPayments = monthlyCampaigns.Count(),
                    totalCampaignsMoney = monthlyCampaigns.Sum(p => p.Amount)
                });
            }

            return result;
        }

        public async Task<UserChart> GetUserChart(int? year)
        {
            var users = await _userRepository.GetAllNewsUserInYear(year);

            var result = new UserChart();

            if (users == null || !users.Any())
                return result;

            result.totalNewUsers = users.Count;
            result.totalNewBrands = users.Count(u => u.Role.RoleName == "Brand");
            result.totalNewInfluencers = users.Count(u => u.Role.RoleName == "Influencer");

            // group by tháng
            var grouped = users
                .GroupBy(u => u.CreatedAt.Month)
                .ToList();

            for (int month = 1; month <= 12; month++)
            {
                var monthGroup = grouped.FirstOrDefault(g => g.Key == month);

                var newBrands = monthGroup?.Count(u => u.Role.RoleName == "Brand") ?? 0;
                var newInfluencers = monthGroup?.Count(u => u.Role.RoleName == "Influencer") ?? 0;

                result.userPerMonths.Add(new UserPerMonth
                {
                    month = month,
                    newBrands = newBrands,
                    newInfluencers = newInfluencers
                });
            }

            return result;
        }

        public async Task BanUser(string uid)
        {
            var user = await _userRepository.GetByIdAsync(uid);
            if (user.Status == UserStatus.Banned)
                throw new Exception("User is already banned.");

            await _userRepository.UpdateUserStatus(uid, UserStatus.Banned);
        }

        public async Task UnBanUser(string uid)
        {
            var user = await _userRepository.GetByIdAsync(uid);
            if (user.Status == UserStatus.Active)
                throw new Exception("User is already Active.");

            await _userRepository.UpdateUserStatus(uid, UserStatus.Active);
        }

        public async Task<List<PaymentDTO>> GetAllPayment(int? year, PaymentStatus? status, PaymentType? type)
        {
            var payments = await _paymentRepository.GetAllPaymentByYearTypeAndStatus(year, type, status);
            return _mapper.Map<List<PaymentDTO>>(payments);
        }

        public async Task<PaymentDetailDTO> GetPaymentDetail(long paymentId)
        {
            var payment = await _paymentRepository.GetPaymentDetail(paymentId);
            return _mapper.Map<PaymentDetailDTO>(payment);
        }
    }
}
