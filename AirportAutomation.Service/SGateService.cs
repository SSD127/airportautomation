using AirportAutomation.Core.Data;
using AirportAutomation.Core.Entities;
using AirportAutomation.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Service
{
    public class SGateService : IGateService
    {
        public string GetAllGates(out List<Gate> gates)
        {
            string? hata = null;
            gates = new List<Gate>();

            try
            {
                using var context = new AirportDbContext();
                gates = context.Gates.AsNoTracking().OrderBy(g => g.Code).ToList();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }

        public string UpdateGateStatus(string code, string status, string currentFlightNumber)
        {
            string? hata = null;

            try
            {
                using var context = new AirportDbContext();
                var gate = context.Gates.FirstOrDefault(g => g.Code == code);
                if (gate == null)
                {
                    return "Kapı bulunamadı.";
                }

                gate.Status = status;
                gate.CurrentFlightNumber = currentFlightNumber;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }
    }
}

