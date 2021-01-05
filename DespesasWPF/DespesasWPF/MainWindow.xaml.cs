using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Interop;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using DespesasLibrary;
using DespesasWPF.ExpenseSOAP;
using Microsoft.Identity.Client;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace DespesasWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // API Endpoint to MS Graph
        private const string GraphApiEndpoint = "https://graph.microsoft.com/v1.0/me";
        private readonly string[] _scopes = {"user.read"};

        // Variáveis para organização das colunas da tabela de Despesas
        private GridViewColumnHeader _listViewSortCol;
        private SortAdorner _listViewSortAdorner;

        private User UtilizadorLigado { get; } = new User();
        private ObservableCollection<Expense> Despesas { get; set; }
        private decimal TotalEur { get; set; }
        private decimal TotalUsd { get; set; }

        private ExpenseSOAPClient soapClient { get; set; }
        private Api api { get; set; }


        private decimal EuroEDolar { get; set; } // Um euro são x dolares
        private decimal DolarEEuro { get; set; } // Um dolar são y euros

        /// <summary>
        ///     Inicialização da MainWindow.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Despesas = new ObservableCollection<Expense>();
            soapClient = new ExpenseSOAPClient();
            TotalEur = 0;
            TotalUsd = 0;
        }

        /// <summary>
        /// Inicia sessão de um Utilizador.
        /// <para>Chama AcquireToken para obter o Token de inicio de sessão.</para>
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

            UtilizadorLigado.EmailSha = Hash.getHashSha256(emailUtilizadorLigadoTemp);
            api = new Api(UtilizadorLigado.EmailSha);

            // Get rates
            EuroEDolar = api.getUsdRatesToEuro();
            DolarEEuro = api.getEuroRatesToUsd();

            if (!api.HasUser())
            {
                soapClient.AddUser(UtilizadorLigado.EmailSha, "EUR");
            }

            UtilizadorLigado.SetUser();

            api.GetExpenses().ForEach(despesa => { Despesas.Add(despesa); });

            // Preenchimento de dados na MainWindow
            LabelUtilizadorLigado.Content = "";
            LabelUtilizadorLigado.Content += nomeTemp + " (" + emailUtilizadorLigadoTemp + ")";

            // Preencher e organizar a tabela de Despesas
            DespesasTable.ItemsSource = Despesas;
            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(DespesasTable.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Data", ListSortDirection.Descending));

            // Calcular e mostrar o total
            foreach (Expense expense in Despesas)
            {
                TotalEur += expense.ValEur;
                TotalUsd += expense.ValUsd;
            }

            if (UtilizadorLigado.MoedaPadrao == "EUR")
            {
                RadioButtonEur.IsChecked = true;
                LabelTotal.Content = TotalEur + "€";
            }
            else
            {
                RadioButtonUsd.IsChecked = true;
                LabelTotal.Content = TotalUsd + "$";
            }

            EntrarPanel.Visibility = Visibility.Collapsed;
            DespesasPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Realiza pedido HTTP GET para obter a informação relativa à conta que iniciou sessão.
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
        ///     Faz Logout do Utilizador atual.
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

        /// <summary>
        ///     Organizador de Despesas.
        ///     <para>Deteta o click na head da coluna da tabela e ativa a organização nessa coluna.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DespesasTableColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string sortBy = column?.Tag.ToString();
            if (_listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(_listViewSortCol)?.Remove(_listViewSortAdorner);
                DespesasTable.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (_listViewSortCol == column && _listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            _listViewSortCol = column;
            _listViewSortAdorner = new SortAdorner(_listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(_listViewSortCol ?? throw new InvalidOperationException())
                ?.Add(_listViewSortAdorner);
            DespesasTable.Items.SortDescriptions.Add(
                new SortDescription(sortBy ?? throw new InvalidOperationException(), newDir));
        }

        /// <summary>
        ///     Função que é executada sempre que o RadioButton do EUR é selecionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonEur_OnChecked(object sender, RoutedEventArgs e)
        {
            // Esta verificação é util no login porque na construção da janela ele dispara o evento da moeda padrão do utilizador
            if (UtilizadorLigado.MoedaPadrao == "EUR") return;
            if (soapClient.UpdateUser(UtilizadorLigado.EmailSha, "EUR"))
            {
                UtilizadorLigado.MoedaPadrao = "EUR";
                LabelTotal.Content = TotalEur + "€";
            }
        }

        /// <summary>
        ///     Função que é executada sempre que o RadioButton do USD é selecionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonUsd_OnChecked(object sender, RoutedEventArgs e)
        {
            // Esta verificação é util no login porque na construção da janela ele dispara o evento da moeda padrão do utilizador
            if (UtilizadorLigado.MoedaPadrao == "USD") return;

            if (soapClient.UpdateUser(UtilizadorLigado.EmailSha, "USD"))
            {
                UtilizadorLigado.MoedaPadrao = "USD";
                LabelTotal.Content = TotalUsd + "$";
            }
        }

        /// <summary>
        ///     Função executada sempre que um botão de editar é clicado. Apenas preenche a informação da Despesa nos campos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEditarDespesa_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            // Verifica se o CommandParameter é uma Despesa e guarda-a
            if (!(button?.CommandParameter is Expense despesa)) return;

            TextBoxId.Text = despesa.Id;
            TextBoxNome.Text = despesa.Nome;
            TextBoxDescricao.Text = despesa.Descricao;
            DatePickerData.Text = despesa.DataHoraCriacao.ToString(CultureInfo.CurrentCulture);
            if (UtilizadorLigado.MoedaPadrao == "EUR")
            {
                TextBoxValorEur.Text = despesa.ValEur.ToString(CultureInfo.CurrentCulture);
                TextBoxValorUsd.Text = "";
            }
            else
            {
                TextBoxValorUsd.Text = despesa.ValUsd.ToString(CultureInfo.CurrentCulture);
                TextBoxValorEur.Text = "";
            }
        }

        /// <summary>
        ///     Função executada sempre que um botão de apagar é clicado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonApagarDespesa_OnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            // Verifica se o CommandParameter é uma Despesa e guarda-a
            if (!(button?.CommandParameter is Expense despesa)) return;

            if (api.DeleteExpense(int.Parse(despesa.Id)))
            {
                Despesas.Remove(despesa);
                TotalEur -= despesa.ValEur;
                TotalUsd -= despesa.ValUsd;
            }
            else
            {
                MessageBox.Show("Occoreu um erro!");
            }
        }

        /// <summary>
        ///     Função executada sempre que o botão de Criação/Edição de Despesa é clicado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCriarEditar_OnClick(object sender, RoutedEventArgs e)
        {
            bool statusEditCreate = false;

            if (TextBoxNome.Text == "" || TextBoxDescricao.Text == "" || DatePickerData.Text == "" ||
                TextBoxValorEur.Text == "" && TextBoxValorUsd.Text == "")
            {
                MessageBox.Show(
                    "Por favor preencha todos os campos. No caso do Valor da Despesa, preencha apenas um campo.",
                    "Alerta");
            }
            else if (TextBoxId.Text == "")
            {
                decimal valEur = 0, valUsd = 0;
                if (TextBoxValorEur.Text == "")
                {
                    // Se o campo do Valor em € não estiver preenchido
                    valUsd = decimal.Parse(TextBoxValorUsd.Text);
                    valEur = valUsd * DolarEEuro;
                }
                else if (TextBoxValorUsd.Text == "")
                {
                    // Se o campo do Valor em $ não estiver preenchido
                    valEur = decimal.Parse(TextBoxValorEur.Text);
                    valUsd = valEur * EuroEDolar;
                }
                else
                {
                    // Se os dois campos forem preenchidos
                    valEur = decimal.Parse(TextBoxValorEur.Text);
                    valUsd = decimal.Parse(TextBoxValorUsd.Text);
                }

                string dateValue = DatePickerData.Text + " " + DateTime.Now.ToString("HH:mm:ss");
                statusEditCreate = soapClient.AddExpense(TextBoxNome.Text, TextBoxDescricao.Text,
                    DateTime.Parse(dateValue), valEur, valUsd,
                    UtilizadorLigado.EmailSha);

                if (statusEditCreate)
                {
                    Despesas.Add(new Expense(api.GetLastIdFromTable("despesas").ToString(), TextBoxNome.Text,
                        TextBoxDescricao.Text,
                        DateTime.Parse(dateValue), valEur, valUsd, UtilizadorLigado.EmailSha));
                    TotalEur += valEur;
                    TotalUsd += valUsd;
                    if (UtilizadorLigado.MoedaPadrao == "EUR")
                    {
                        LabelTotal.Content = TotalEur + "€";
                    }
                    else
                    {
                        LabelTotal.Content = TotalUsd + "$";
                    }
                }
            }
            else
            {
                decimal valEur = 0, valUsd = 0;
                if (TextBoxValorEur.Text == "")
                {
                    // Se o campo do Valor em € não estiver preenchido
                    valUsd = decimal.Parse(TextBoxValorUsd.Text);
                    valEur = valUsd * DolarEEuro; 
                }
                else if (TextBoxValorUsd.Text == "")
                {
                    // Se o campo do Valor em $ não estiver preenchido
                    valEur = decimal.Parse(TextBoxValorEur.Text);
                    valUsd = valEur * EuroEDolar;
                }
                else
                {
                    // Se os dois campos forem preenchidos
                    valEur = decimal.Parse(TextBoxValorEur.Text);
                    valUsd = decimal.Parse(TextBoxValorUsd.Text);
                }

                MessageBox.Show("Editar Despesa na BD");
                statusEditCreate = soapClient.UpdateExpense(TextBoxId.Text,
                    TextBoxNome.Text,
                    TextBoxDescricao.Text,
                    DateTime.Parse(DatePickerData.Text),
                    valEur,
                    valUsd,
                    UtilizadorLigado.EmailSha);
                
                Expense despesaEditar = Despesas.FirstOrDefault(d => d.Id == TextBoxId.Text);
                if (despesaEditar != null && statusEditCreate)
                {
                    Despesas.Remove(despesaEditar);
                    TotalEur -= despesaEditar.ValEur;
                    TotalUsd -= despesaEditar.ValUsd;
                    Despesas.Add(new Expense(TextBoxId.Text, TextBoxNome.Text, TextBoxDescricao.Text,
                        DateTime.Parse(DatePickerData.Text), valEur, valUsd, UtilizadorLigado.EmailSha));
                    TotalEur += valEur;
                    TotalUsd += valUsd;
                    if (UtilizadorLigado.MoedaPadrao == "EUR")
                    {
                        LabelTotal.Content = TotalEur + "€";
                    }
                    else
                    {
                        LabelTotal.Content = TotalUsd + "$";
                    }
                }
            }
            
            if (statusEditCreate)
            {
                TextBoxId.Text = "";
                TextBoxNome.Text = "";
                TextBoxDescricao.Text = "";
                DatePickerData.Text = "";
                TextBoxValorEur.Text = "";
                TextBoxValorUsd.Text = "";
            }
        }

        /// <summary>
        ///     Função executada sempre que o botão de Cancelar Criação/Edição de Despesa é clicado.
        ///     <para>Limpa os campos de Criação/Edição de Despesas.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelar_OnClick(object sender, RoutedEventArgs e)
        {
            TextBoxId.Text = "";
            TextBoxNome.Text = "";
            TextBoxDescricao.Text = "";
            DatePickerData.Text = "";
            TextBoxValorEur.Text = "";
            TextBoxValorUsd.Text = "";
        }

        /// <summary>
        ///     Verifica os carateres que são introduzidos nos campos de Valor e deixa inserir um "." e ilimitados números
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxValor_OnKeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica se foi introduzido um "."
            if (Regex.IsMatch(textBox?.Text ?? throw new InvalidOperationException(),
                "[\\.]"))
            {
                // Se foi introduzido um "." este será retirado se já existir outro
                int pontos = (textBox.Text).Count(c => c == '.');
                if (pontos > 1)
                {
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }

            /*
             Verifica se foi introduzido um número ou ".", se sim termina a execução do procedimento (o ponto já foi 
             tratado em cima, logo tem de ser deixado passar aqui senão vão ser retirados carateres a mais)
            */
            if (!Regex.IsMatch(textBox?.Text ?? throw new InvalidOperationException(),
                "[^0-9.]")) return;
            textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
            textBox.CaretIndex = textBox.Text.Length;
        }
    }
}