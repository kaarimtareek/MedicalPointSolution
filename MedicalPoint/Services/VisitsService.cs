﻿using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

using static Azure.Core.HttpHeader;

namespace MedicalPoint.Services
{
    public interface IVisitsService
    {
        Task<OperationResult<Visit>> ChangeStatus(int visitId, int userId, string status, bool forceChange = false, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Create(int userId, int patientId, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Delete(int visitId, int userId, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Edit(int visitId, int userId,  string notes, DateTime? exitTime = null, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, DateTime? followDate = null, bool hasFollowingVisit = false, CancellationToken cancellationToken = default);
        Task<Visit> Get(int visitId, CancellationToken cancellationToken = default);
        Task<PaginatedList<Visit>> GetAll(int pageNumber = 1, int pageSize = 20, int? doctorId = null, int? patientId = null, DateTime? from = null, DateTime? to = null, string? type = null, int? clinicId = null, string? searchValue = null, CancellationToken cancellationToken = default);
        Task<VisitRest> GetVisitRest(int visitId, CancellationToken cancellationToken = default);
        Task<List<LookupVisitRestType>> GetVisitRestTypes(CancellationToken cancellationToken = default);
        Task<PaginatedList<Visit>> GetVisitsMedicines( string searchValue, DateTime? date, bool notGivenOnly = true, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<List<Visit>> GetVisitsThatGivenMedicines(DateTime date, CancellationToken cancellationToken = default);
        Task<bool> IsVisitHasRest(int visitId, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> WriteDiagnosis(int visitId, int userId, string diagnosis, bool forceChange = false, CancellationToken cancellationToken = default);
    }

    public class VisitsService : IVisitsService
    {
        private readonly ApplicationDbContext _context;

        public VisitsService(ApplicationDbContext context)
        {
            _context = context;
        }
        

        public async Task<OperationResult<Visit>> Create(int userId, int patientId, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            if(!clinicId.HasValue ||  !await _context.Clinics.AnyAsync(x=> x.Id == clinicId))
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.ClinicNotFound);

            }
            if (string.IsNullOrEmpty(type) || !ConstantVisitType.ALL.Contains(type))
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitTypeNotFound);

            }
            if (QueryValidator.PatientHasAlreadyActiveVisit(_context, patientId))
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.PatientHasAlreadyActiveVisit);
            }
            var numberOfVisitsToday = QueryFinder.GetVisitsCountForDay(_context);
            var currentVisitNumber = ++numberOfVisitsToday;
            string visitNumber = $"{DateTime.Today:yyyyMMdd}{currentVisitNumber:D3}";
            //we can add checks for the existence of clinic & doctor but will skip it for simplicity 
            var visit = new Visit
            {
                ClinicId = clinicId,
                DoctorId = doctorId,
                CreatedAt = DateTime.Now,
                PatientId = patientId,
                PreviousVisitId = previousVisitId,
                Status = ConstantVisitStatus.IN_RECIEPTION,
                Type = type ?? string.Empty,
                VisitTime = visitTime ?? DateTime.Now,
                VisitNumber = visitNumber,
                Diagnosis = string.Empty,
                Notes = string.Empty,
                RegisteredUserId = userId,
                
                
            };
            await _context.Visits.AddAsync(visit, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var visitHistory = new VisitHistory
            {
                VisitId = visit.Id,
                CreatedAt = DateTime.Now,
                UserId = userId,
                ClinicId = clinicId,
                Type = type ?? string.Empty,
                VisitTime = visitTime ?? DateTime.Now,
                DoctorId = doctorId,
                PreviousVisitId = previousVisitId,
                PatientId = patientId,
                Status = ConstantVisitStatus.IN_RECIEPTION,
                VisitNumber = visitNumber,
                Notes= string.Empty,
                Diagnosis= string.Empty,

            };
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

        public async Task<OperationResult<Visit>> Edit(int visitId, int userId,  string notes, DateTime? exitTime = null, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, DateTime? followDate = null, bool hasFollowingVisit = false, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            visit.Diagnosis = "";
            visit.Notes = notes??"";
            visit.ExitTime = exitTime;
            visit.VisitTime = visitTime ?? visit.VisitTime;
            visit.ClinicId = clinicId;
            visit.DoctorId = doctorId;
            visit.FollowingVisitDate = followDate;
            visit.PreviousVisitId = previousVisitId;
            visit.Type = type ?? string.Empty;
            visit.HasFollowingVisit = hasFollowingVisit;
            visit.DoctorId = doctorId;

            var visitHistory = new VisitHistory
            {
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                ClinicId = clinicId,
                Type = type?? string.Empty,
                VisitTime = visitTime,
                Diagnosis = "",
                Notes = notes??"",
                ExitTime = exitTime,
                DoctorId = doctorId,
                HasFollowingVisit = hasFollowingVisit,
                PreviousVisitId = previousVisitId,
                FollowingVisitDate = followDate,
                
            };

            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

        public async Task<OperationResult<Visit>> ChangeStatus(int visitId, int userId, string status, bool forceChange = false, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitNotFound);
            }
            if (!forceChange && !ConstantVisitStatus.CanChangeStatus(visit.Status, status))
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.CannotChangeVisitStatus);
            }
            if(status == ConstantVisitStatus.TAKING_MEDICINE)
            {
                if (string.IsNullOrEmpty(visit.Diagnosis))
                {
                    return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitDiagnosisIsNotWritten);

                }
                if (!await _context.VisitMedicines.AnyAsync(x => x.VisitId == visitId))
                    status = ConstantVisitStatus.FINISHED;
            }
            visit.Status = status;
           
            if(status == ConstantVisitStatus.FINISHED)
            {
                if(string.IsNullOrEmpty(visit.Diagnosis))
                {
                    return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitDiagnosisIsNotWritten);

                }
                visit.ExitTime = DateTime.Now;
            }    
            var visitHistory = new VisitHistory
            {
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                VisitTime = visit.VisitTime,
                 Status = status,
                Diagnosis =  "",
                Notes = "",
                Type = "",
                VisitNumber = "",
            };
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }
        
        public async Task<OperationResult<Visit>> WriteDiagnosis(int visitId, int userId, string diagnosis, bool forceChange = false, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(diagnosis) || string.IsNullOrWhiteSpace(diagnosis.Trim()))
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitDiagnosisIsNotWritten);
            }
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitNotFound);
            }
            if (!forceChange && !visit.CanEditVisit(forceChange))
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.CannotChangeVisitStatus);
            }
            visit.Diagnosis = diagnosis;
            visit.DoctorId = userId;
            if(visit.Status == ConstantVisitStatus.IN_RECIEPTION)
            {
                visit.Status = ConstantVisitStatus.IN_CLINIC_DIAGNOSIS;
            }
            var visitHistory = new VisitHistory
            {
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                 Status = visit.Status,
                Diagnosis =  diagnosis,
                Notes = "",
                Type = "",
                VisitNumber = "",
            };
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }
        
       
        public async Task<OperationResult<Visit>> Delete(int visitId, int userId, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(ConstantMessageCodes.VisitNotFound);
            }

            visit.IsDeleted = true;
            var visitHistory = new VisitHistory
            {
                 VisitId = visitId,
                 IsDeleted = true,
                 CreatedAt = DateTime.Now,
                 UserId = userId,
                Diagnosis = "",
                Notes = "",
                Type = "",
                VisitNumber = "",
            };

            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }
        public async Task<PaginatedList<Visit>> GetAll(int pageNumber =1, int pageSize = 20, int? doctorId = null, int? patientId = null, DateTime? from = null, DateTime? to = null, string? type = null, int? clinicId = null, string? searchValue = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Visits
                .Include(x=> x.Doctor)
                .Include(x=> x.Patient)
                    .ThenInclude(x=> x.Degree)
                 .Include(x=> x.Clinic)
                .AsNoTracking().AsQueryable();

            if (doctorId != null)
            {
                query = query.Where(x => x.DoctorId == doctorId);
            }
            if (patientId != null)
            {
                query = query.Where(x => x.PatientId == patientId);
            }
            if (from != null)
            {
                from = from.Value.Date;
                query = query.Where(x => x.VisitTime >= from);
            }
            if (to != null)
            {
                to = to.Value.Date;
                query = query.Where(x => x.VisitTime <= to);
            }
            if (type != null)
            {
                query = query.Where(x => x.Type == type);
            }
            if (clinicId != null)
            {
                query = query.Where(x => x.ClinicId == clinicId);
            }
            if(searchValue != null)
            {
                query = query.Where(x=> x.Patient.Name
                .Contains(searchValue) || x.Patient.MilitaryNumber.Contains(searchValue) || x.Patient.NationalNumber.Contains(searchValue) || x.Patient.SaryaNumber.Contains(searchValue) || x.Patient.GeneralNumber.Contains(searchValue));
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            var result = await PaginatedList<Visit>.CreateAsync( query, pageNumber, pageSize);
            return result;
        }
        public async Task<PaginatedList<Visit>> GetVisitsMedicines(string searchValue, DateTime? date, bool notGivenOnly = true, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = _context.Visits
                .Include(x=> x.Doctor)
                .Include(x=> x.Patient)
                    .ThenInclude(x=> x.Degree)
                 .Include(x=> x.Clinic)
                .AsNoTracking().Where(x => x.Medicines.Count > 0);
            if(!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x=> x.Patient.GeneralNumber == searchValue || x.Patient.SaryaNumber == searchValue || x.Patient.Name.Contains(searchValue));
            }
            DateTime endDate;
            if(date.HasValue)
            {
                date = date.Value.Date;
                endDate =date.Value.Date.AddDays(1);
            }
            else
            {
                date = DateTime.Today.Date;
                endDate = date.Value.AddDays(1);
            }
            if(notGivenOnly)
            {
                query = query.Where(x => !x.IsMedicinesGiven && x.Status == ConstantVisitStatus.TAKING_MEDICINE );
            }
            query = query.Where(x => x.VisitTime >= date.Value && x.VisitTime <= endDate);

            var result = await PaginatedList<Visit>.CreateAsync(query, pageNumber, pageSize);
            return result;
        }
        public async Task<Visit> Get(int visitId, CancellationToken cancellationToken = default)
        {
            var visit = await _context.Visits.AsNoTracking()
                .Include(x=> x.Clinic)
                .Include(x=> x.Doctor)
                .Include(x=> x.Patient)
                    .ThenInclude(x=> x.Degree)
                .Include(x=> x.Patient)
                    .ThenInclude(x => x.RegisteredUser)
                .Include(x=> x.Medicines)
                    .ThenInclude(x=> x.Medicine)
                .Include(x=> x.PreviousVisit)
                .Include(x=> x.Images)
                .Include(x=> x.History.OrderByDescending(x=> x.CreatedAt))
                .FirstOrDefaultAsync(x=> x.Id == visitId, cancellationToken);
           

            return visit;
        }
        public async Task<bool> IsVisitHasRest(int visitId, CancellationToken cancellationToken = default)
        {
            var visit = await _context.VisitRests.AsNoTracking()
                .FirstOrDefaultAsync(x=> x.VisitId == visitId, cancellationToken);
           

            return visit !=null;
        }
        public async Task<VisitRest> GetVisitRest(int visitId, CancellationToken cancellationToken = default)
        {
            var visit = await _context.VisitRests.AsNoTracking()
                .Include(x=> x.RestType)
                .FirstOrDefaultAsync(x=> x.VisitId == visitId, cancellationToken);
            return visit;
        }
        public async Task<List<LookupVisitRestType>> GetVisitRestTypes(CancellationToken cancellationToken = default)
        {
            var visit = await _context.LookupVisitRestTypes.AsNoTracking()
                .ToListAsync(cancellationToken);
            return visit;
        }

        public async Task<List<Visit>> GetVisitsThatGivenMedicines(DateTime date, CancellationToken cancellationToken = default)
        {
            date = date.Date;
            var endDate = date.AddDays(1);
            var query = _context.Visits
                .Include(x => x.Doctor)
                .Include(x => x.Patient)
                    .ThenInclude(x => x.Degree)
                 .Include(x => x.Clinic)
                .AsNoTracking().AsQueryable();


            var result = await query.Where(x => x.CreatedAt >=date && x.CreatedAt <=endDate && x.Medicines.Count > 0 && x.IsMedicinesGiven).OrderByDescending(x=> x.CreatedAt).ToListAsync(cancellationToken);
            return result;
        }
    }
}
