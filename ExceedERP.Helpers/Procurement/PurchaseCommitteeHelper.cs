using System;
using System.Linq;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Procurement
{
    public static class PurchaseCommitteeHelper
    {
        public static void ApplyToProformaTeam(ExceedDbContext db, int proformaTeamId, string createdBy)
        {
            var team = db.ProformaTeams.Find(proformaTeamId);
            if (team == null || team.StandingCommitteeApplied)
                return;

            var hasMembers = db.ProformaTeamMembers.Any(x => x.ProformaTeamId == proformaTeamId);
            if (!hasMembers)
            {
                var standing = db.PurchaseCommitteeMembers
                    .Where(x => x.CommitteeType == PurchaseCommitteeType.Proforma)
                    .ToList();

                if (!standing.Any())
                    return;

                foreach (var member in standing)
                {
                    db.ProformaTeamMembers.Add(new ProformaTeamMember
                    {
                        ProformaTeamId = proformaTeamId,
                        EmployeeId = member.EmployeeId,
                        Title = member.Title,
                        Signed = false,
                        DateCreated = DateTime.Today,
                        CreatedBy = createdBy
                    });
                }
            }

            team.StandingCommitteeApplied = true;
            db.SaveChanges();
        }

        public static void ApplyToLotTeam(ExceedDbContext db, int lotTeamId, string createdBy)
        {
            var team = db.LotTeams.Find(lotTeamId);
            if (team == null || team.StandingCommitteeApplied)
                return;

            var hasMembers = db.LotTeamMembers.Any(x => x.LotTeamId == lotTeamId);
            if (!hasMembers)
            {
                var standing = db.PurchaseCommitteeMembers
                    .Where(x => x.CommitteeType == PurchaseCommitteeType.Tender)
                    .ToList();

                if (!standing.Any())
                    return;

                foreach (var member in standing)
                {
                    db.LotTeamMembers.Add(new LotTeamMember
                    {
                        LotTeamId = lotTeamId,
                        EmployeeId = member.EmployeeId,
                        Title = member.Title,
                        Signed = false,
                        DateCreated = DateTime.Today,
                        CreatedBy = createdBy
                    });
                }
            }

            team.StandingCommitteeApplied = true;
            db.SaveChanges();
        }
    }
}
