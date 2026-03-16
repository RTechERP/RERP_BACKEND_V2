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
            HRRecruitmentCandidate currentUser = new HRRecruitmentCandidate();

            var props = typeof(HRRecruitmentCandidate).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;

                var value = claims.TryGetValue(prop.Name.ToLower(), out var rawValuea);
                string claimKey = prop.Name.ToLower();
                // Ưu tiên lấy từ 'candidateid' nếu prop là 'id'
                if (claimKey == "id" && claims.ContainsKey("candidateid"))
                {
                    claimKey = "candidateid";
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
    }
}