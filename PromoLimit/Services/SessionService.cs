using Microsoft.AspNetCore.Identity;
using PromoLimit.Models;
using PromoLimit.States;

namespace PromoLimit.Services
{
    public class SessionService
    {
        private readonly UserDataService _userDataService;
        private readonly JsonWebToken _jsonWebToken;
        private readonly IPasswordHasher<User> _passwordHasher;

        public SessionService(IServiceProvider provider)
        {
            _userDataService = provider.GetRequiredService<UserDataService>();
            _passwordHasher = provider.GetRequiredService<IPasswordHasher<User>>();
            _jsonWebToken = provider.GetRequiredService<JsonWebToken>();
        }

        public bool IsSignedIn(HttpContext context)
        {
            return SessionManager.IsSignedIn(context);
        }

        public async Task<User?> GetLoggedinUser(HttpContext context)
        {
            if (!IsSignedIn(context))
            {
                return null;
            }
            if (!Guid.TryParse(context.Session.GetString("uuid"), out Guid guid))
            {
                return null;
            }

            var aaa = await _userDataService.GetByUuid(guid);
            return aaa;
        }

        public async Task<RegisterInfo> RegisterNewUser(string nome, string username, string password, string confirmation)
        {
            RegisterInfo ri = new();
            var funcionário = await _userDataService.GetByUsername(username);
            if (funcionário is not null)
            {
                ri.ErrorMessage = "Usuário já existe";
                return ri;
            }

            if (password != confirmation)
            {
                ri.ErrorMessage = "Senhas não conferem";
                return ri;
            }
            User novoFuncionário = new();
            novoFuncionário.Login = username;
            novoFuncionário.Nome = nome;
            novoFuncionário.Auth = _passwordHasher.HashPassword(novoFuncionário, password);
            novoFuncionário.Uuid = Guid.NewGuid();
            novoFuncionário.Ativo = false;
            await _userDataService.AddOrUpdateUser(novoFuncionário);
            return ri;
        }

        public async Task<LoginInfo> LogIn(string username, string password)
        {
            LoginInfo loginInfo = new();
            var funcionario = await _userDataService.GetByUsername(username);
            if (funcionario is null)
            {
                loginInfo.ErrorMessage = "Usuário ou senha incorreta";
                return loginInfo;
            }
            else if (!funcionario.Ativo)
            {
                loginInfo.ErrorMessage = "Usuário inativo. Verifique com seu supervisor.";
                return loginInfo;
            }
            else
            {
                ApiKeyInfo keyInfo;
                switch (_passwordHasher.VerifyHashedPassword(funcionario, funcionario.Auth, password))
                {
                    case PasswordVerificationResult.Failed:
                        loginInfo.ErrorMessage = "Usuário ou senha incorreta";
                        break;
                    case PasswordVerificationResult.Success:
                        keyInfo = new ApiKeyInfo
                        {
                            uuid = funcionario.Uuid.ToString(), 
                            //setorId = 0, gerencia = funcionario.Gerencia, 
                            displayName = funcionario.Nome ?? "Sem Nome"
                        };
                        loginInfo.Apikey = _jsonWebToken.EncodeToken(keyInfo);
                        loginInfo.KeyInfo = keyInfo;
                        break;
                    case PasswordVerificationResult.SuccessRehashNeeded:
                        keyInfo = new ApiKeyInfo
                        {
                            uuid = funcionario.Uuid.ToString(), 
                            //setorId = 0, gerencia = funcionario.Gerencia, 
                            displayName = funcionario.Nome ?? "Sem Nome"
                        };
                        loginInfo.Apikey = _jsonWebToken.EncodeToken(keyInfo);
                        loginInfo.KeyInfo = keyInfo; Console.WriteLine("Rehash needed");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return loginInfo;
        }
        public class LoginInfo
        {
            public string Apikey { get; set; }
            public string ErrorMessage { get; set; }
            public string RefreshToken { get; set; }
            public ApiKeyInfo KeyInfo { get; set; }
        }

        public class RegisterInfo
        {
            public string ErrorMessage { get; set; }
        }
    }
}
