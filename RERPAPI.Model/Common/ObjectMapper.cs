using RERPAPI.Model.DTO;
using RERPAPI.Model.Entities;

namespace RERPAPI.Model.Common
{
    public static class ObjectMapper
    {
        public static TTarget MapTo<TTarget>(object source) where TTarget : new()
        {
            TTarget target = new TTarget();

            try
            {
                var sourceProps = source.GetType().GetProperties();
                var targetProps = typeof(TTarget).GetProperties();

                foreach (var sp in sourceProps)
                {
                    var tProp = targetProps.FirstOrDefault(x => x.Name == sp.Name && x.PropertyType == sp.PropertyType);
                    if (tProp != null && tProp.CanWrite) tProp.SetValue(target, sp.GetValue(source));
                }

                return target;
            }
            catch (Exception ex)
            {
                return target;
                throw;
            }
        }

        public static CurrentUser GetCurrentUser(Dictionary<string, string> claims)
        {
            CurrentUser currentUser = new CurrentUser();
            if (claims == null || (claims.TryGetValue("iscandidate", out var isCandidate) && isCandidate == "true"))
            {
                return currentUser; // Nếu là Token ứng viên thì không map vào CurrentUser nhân viên
            }

            var props = typeof(CurrentUser).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;

                var value = claims.TryGetValue(prop.Name.ToLower(), out var rawValuea);

                if (claims.TryGetValue(prop.Name.ToLower(), out var rawValue))
                {
                    try
                    {
                        object? parsedValue = prop.PropertyType switch
                        {
                            Type t when t == typeof(string) => rawValue,
                            Type t when t == typeof(int) || t == typeof(int?) => int.TryParse(rawValue, out var i) ? i : 0,
                            Type t when t == typeof(bool) || t == typeof(bool?) => bool.TryParse(rawValue, out var b) ? b : false,
                            Type t when t == typeof(DateTime) || t == typeof(DateTime?) => DateTime.TryParse(rawValue, out var d) ? d : null,
                            _ => null
                        };

                        if (parsedValue != null) prop.SetValue(currentUser, parsedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{prop.Name}\r\n{ex.Message}\r\n{ex.ToString()}");
                    }
                }
            }

            return currentUser;
        }
        public static HRRecruitmentCandidate GetCurrentCandidate(Dictionary<string, string> claims)
        {
            HRRecruitmentCandidate currentCandidate = new HRRecruitmentCandidate();
            if (claims == null || (claims.TryGetValue("iscandidate", out var isCandidate) && isCandidate != "true"))
            {
                return currentCandidate; // Nếu không phải Token ứng viên thì không map dữ liệu
            }

            var props = typeof(HRRecruitmentCandidate).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;

                string claimKey = prop.Name.ToLower();

                if (claims.ContainsKey("app_" + claimKey))
                {
                    claimKey = "app_" + claimKey;
                }
                // Ưu tiên lấy từ 'candidateid' nếu prop là 'id'
                if (claimKey == "id")
                {
                    if (claims.ContainsKey("candidateid"))
                    {
                        claimKey = "candidateid";
                    }
                    else
                    {
                        // Nếu không có candidateid thì không được map vào ID của candidate
                        // để tránh nhầm lẫn với UserID (hệ thống)
                        continue;
                    }
                }
                if (prop.Name == "FullName")
                {
                    if (claims.TryGetValue("fullname", out var fullName) ||
                        claims.TryGetValue("app_fullname", out fullName))
                    {
                        prop.SetValue(currentCandidate, fullName);
                        continue;
                    }

                 
                    if (claims.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var name))
                    {
                        prop.SetValue(currentCandidate, name);
                        continue;
                    }
                }
                if (claims.TryGetValue(claimKey, out var rawValue))
                {
                    try
                    {
                        object? parsedValue = prop.PropertyType switch
                        {
                            Type t when t == typeof(string) => rawValue,
                            Type t when t == typeof(int) || t == typeof(int?) => int.TryParse(rawValue, out var i) ? i : 0,
                            Type t when t == typeof(bool) || t == typeof(bool?) => bool.TryParse(rawValue, out var b) ? b : false,
                            Type t when t == typeof(DateTime) || t == typeof(DateTime?) => DateTime.TryParse(rawValue, out var d) ? d : null,
                            _ => null
                        };

                        if (parsedValue != null) prop.SetValue(currentCandidate, parsedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{prop.Name}\r\n{ex.Message}\r\n{ex.ToString()}");
                    }
                }
            }

            return currentCandidate;
        }
    }
}