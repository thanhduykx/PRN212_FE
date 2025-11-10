using AssignmentPRN212.Models;
using System.Threading.Tasks;

namespace AssignmentPRN212.Services
{
    public class UserService
    {
        private readonly ApiService _api;

        public UserService(ApiService api)
        {
            _api = api;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _api.PostAsync<LoginRequest, LoginResponse>("Auth/login", request);

            if (!string.IsNullOrEmpty(response.Token))
                _api.SetToken(response.Token); // lưu token để gọi API khác

            return response;
        }
    }
}
