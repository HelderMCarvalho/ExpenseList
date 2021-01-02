using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace DespesasWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // API Endpoint to MS Graph
        private const string GraphApiEndpoint = "https://graph.microsoft.com/v1.0/me";
        private readonly string[] _scopes = {"user.read"};

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inicia sessão de um Utilizador
        /// <para>Chama AcquireToken para obter o Token de inicio de sessão</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonEntrar_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationResult authResult = null;
            IPublicClientApplication app = App.PublicClientApp;
            IEnumerable<IAccount> accounts = (await app.GetAccountsAsync()).ToList();
            IAccount firstAccount = accounts.FirstOrDefault();
            try
            {
                authResult = await app.AcquireTokenSilent(_scopes, firstAccount).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent. 
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                Console.WriteLine(@"MsalUiRequiredException: " + ex.Message);
                try
                {
                    authResult = await app.AcquireTokenInteractive(_scopes).WithAccount(accounts.FirstOrDefault())
                        .WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                        .WithPrompt(Prompt.SelectAccount).ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    // Erro ao adquirir Token
                    MessageBox.Show("Erro ao adquirir Token: " + msalex,
                        "DespesasWPF: MainWindow.ButtonEntrar_Click");
                }
            }
            catch (Exception ex)
            {
                // Erro ao adquirir Token Silenciosamente
                MessageBox.Show("Erro ao adquirir Token Silenciosamente: " + ex,
                    "DespesasWPF: MainWindow.ButtonEntrar_Click");
            }

            if (authResult == null) return;
            string nomeTemp = await GetHttpContentWithToken(GraphApiEndpoint, authResult.AccessToken);
            string emailUtilizadorLigadoTemp = authResult.Account.Username;
            MessageBox.Show(emailUtilizadorLigadoTemp, nomeTemp);
            // TextBlockUtilizadorLigado.Text = "";
            // TextBlockUtilizadorLigado.Text += nomeTemp + " (" + emailUtilizadorLigadoTemp + ")";
            EntrarPanel.Visibility = Visibility.Collapsed;
            DespesasPanel.Visibility = Visibility.Visible;
            // var user = TextBlockUtilizadorLigado.Text.Contains("alunos")
            // ? new Utilizador(nomeTemp, emailUtilizadorLigadoTemp, Utilizador.UserType.Aluno)
            // : new Utilizador(nomeTemp, emailUtilizadorLigadoTemp, Utilizador.UserType.Prof);
        }

        /// <summary>
        /// Realiza pedido HTTP GET para obter a informação relativa à conta que iniciou sessão
        /// </summary>
        /// <param name="url">API URL</param>
        /// <param name="token">Token de acesso</param>
        /// <returns>Primeiro e último nome do Utilizador</returns>
        private static async Task<string> GetHttpContentWithToken(string url, string token)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(url, UriKind.Absolute));
                request.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendRequestAsync(request);
                string content = await response.Content.ReadAsStringAsync();
                JObject contentJObject = JObject.Parse(content);
                return contentJObject["givenName"] + " " + (string) contentJObject["surname"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DespesasWPF: MainWindow.GetHttpContentWithToken");
                return ex.Message;
            }
        }

        /// <summary>
        /// Faz Logout do Utilizador atual
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonSair_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<IAccount> accounts = (await App.PublicClientApp.GetAccountsAsync()).ToList();
            if (!accounts.Any()) return;
            try
            {
                await App.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                DespesasPanel.Visibility = Visibility.Collapsed;
                EntrarPanel.Visibility = Visibility.Visible;
            }
            catch (MsalException ex)
            {
                // Erro ao Sair
                MessageBox.Show(ex.Message, "DespesasWPF: MainWindow.ButtonSair_Click");
            }
        }
    }
}