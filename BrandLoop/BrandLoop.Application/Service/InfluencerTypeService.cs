using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class InfluencerTypeService : IInfluencerTypeService
    {
        private readonly IInfluencerTypeRepository _influencerTypeRepository;
        private readonly IMapper _mapper;

        public InfluencerTypeService(IInfluencerTypeRepository influencerTypeRepository, IMapper mapper)
        {
            _influencerTypeRepository = influencerTypeRepository;
            _mapper = mapper;
        }

        public async Task<InfluTypeModel> AddInfluencerTypeAsync(InfluTypeModel influencerType)
        {
            var entity = _mapper.Map<Domain.Entities.InfluencerType>(influencerType);
            var addedEntity = await _influencerTypeRepository.AddInfluencerTypeAsync(entity);
            return _mapper.Map<InfluTypeModel>(addedEntity);
        }

        public async Task<List<InfluTypeModel>> GetAllInfluencerTypesAsync()
        {
            var influencerTypes = await _influencerTypeRepository.GetAllInfluencerTypesAsync();
            return _mapper.Map<List<InfluTypeModel>>(influencerTypes);
        }

        public async Task<InfluTypeModel> GetInfluencerTypeByIdAsync(int id)
        {
            var influencerType = await _influencerTypeRepository.GetInfluencerTypeByIdAsync(id);
            if (influencerType == null)
                throw new KeyNotFoundException($"InfluencerType with ID {id} not found.");

            return _mapper.Map<InfluTypeModel>(influencerType);
        }

        public async Task<InfluTypeModel> UpdateInfluencerTypeAsync(InfluTypeModel influencerType)
        {
            var entity = _mapper.Map<Domain.Entities.InfluencerType>(influencerType);

            // check if the entity exists before updating
            var updateEntity = await _influencerTypeRepository.GetInfluencerTypeByIdAsync(entity.Id);
            if (updateEntity == null)
                throw new KeyNotFoundException($"InfluencerType with ID {influencerType.Id} not found.");

            return _mapper.Map<InfluTypeModel>(await _influencerTypeRepository.UpdateInfluencerTypeAsync(entity));
        }
    }
}
