using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using DespesasLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DespesasWPF
{
    public class Api
    {
        HttpWebRequest request;
        string url;
        private string hashUser;


        public Api(string hashUser)
        {
            this.hashUser = hashUser;
        }

        public void reset()
        {
            url = "https://localhost:44325";
        }


        public List<Expense> GetExpenses()
        {
            reset();
            url += "/Expense/{hashUser}/GetAll";
            url = url.Replace("{hashUser}", hashUser);

            return JsonConvert.DeserializeObject<List<Expense>>(_get());
        }

        public bool DeleteExpense(int id)
        {
            reset();
            url += "/Expense/{hashUser}/Delete/{id}";
            url = url.Replace("{hashUser}", hashUser);
            url = url.Replace("{id}", id.ToString());

            return _delete();
        }

        public bool HasUser()
        {
            reset();
            url += "/Expense/{hashUser}/HasUser";
            url = url.Replace("{hashUser}", hashUser);

            return JsonConvert.DeserializeObject<bool>(_get());
        }


        private string _get()
        {
            // Create request with Updated Url
            request = WebRequest.Create(url) as HttpWebRequest;
            try
            {
                using (var response = request?.GetResponse() as HttpWebResponse)
                {
                    if (response?.StatusCode != HttpStatusCode.OK)
                    {
                        string message = $"HTTP ERROR CODE: {response.StatusCode}";
                        throw new ApplicationException(message);
                    }

                    return new StreamReader(response?.GetResponseStream() ?? throw new InvalidOperationException())
                        .ReadToEnd();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        private bool _delete()
        {
            // Create request with Updated Url
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "DELETE";
            try
            {
                using (var response = request?.GetResponse() as HttpWebResponse)
                {
                    if (response?.StatusCode != HttpStatusCode.OK)
                    {
                        string message = $"HTTP ERROR CODE: {response.StatusCode}";
                        throw new ApplicationException(message);
                    }

                    return true;
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }


        /// <summary>
        /// Devolve o valor atual do dolar equivalente a um euro 
        /// </summary>
        /// <returns></returns>
        public decimal getUsdRatesToEuro()
        {
            url = "https://api.exchangeratesapi.io/latest?base=EUR";
            request = WebRequest.Create(url) as HttpWebRequest;

            return JObject.Parse(_get()).SelectToken("rates").Value<decimal>("USD");
        }

        /// <summary>
        /// Devolve o valor atual do euro equivalente a um dolar 
        /// </summary>
        /// <returns></returns>
        public decimal getEuroRatesToUsd()
        {
            url = "https://api.exchangeratesapi.io/latest?base=USD";
            request = WebRequest.Create(url) as HttpWebRequest;

            return JObject.Parse(_get()).SelectToken("rates").Value<decimal>("EUR");
        }


        public int GetLastIdFromTable(string nomeTabela)
        {
            reset();
            url += "/Expense/{hashUser}/GetLastId/{nomeTabela}";
            url = url.Replace("{hashUser}", hashUser);
            url = url.Replace("{nomeTabela}", nomeTabela);

            // Create request with Updated Url
            request = WebRequest.Create(url) as HttpWebRequest;

            return JsonConvert.DeserializeObject<int>(_get());
        }
    }
}