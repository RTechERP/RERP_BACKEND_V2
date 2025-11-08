using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo.GenericEntity
{
    public class POKHRepo : GenericRepo<POKH>
    {
        RTCContext _context = new RTCContext();

        public POKHRepo(CurrentUser currentUser) : base(currentUser)
        {
        }


        public string GenerateUniqueFileName(string originalFileName)
        {
            string extension = Path.GetExtension(originalFileName);
            string fileName = Path.GetFileNameWithoutExtension(originalFileName);

            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(fileName + DateTime.Now.Ticks);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower() + extension;
            }
        }
        public string FindLastestCode(string customer)
        {
            return _context.POKHs
                    .Where(p => p.POCode != null && p.POCode.Contains(customer))
                    .OrderByDescending(p => p.ID)
                    .Select(p => p.POCode)
                    .FirstOrDefault() ?? string.Empty;
        }
    }
}
