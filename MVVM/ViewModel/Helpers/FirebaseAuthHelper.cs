using NotesApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Windows;
using System.Configuration;
using NotesApp.Properties;

namespace NotesApp.MVVM.ViewModel.Helpers
{
    public class FirebaseAuthHelper
    {
        //Use API Key given by Firebase
        public static string API_KEY = Settings.Default.API_KEY;

        public static async Task<bool> Register(User user)
        {
            using(HttpClient client = new HttpClient())
            {
                var body = new
                {
                    email = user.Username,
                    password = user.Password,
                    returnSecureToken = true
                };

                var bodyJson = JsonSerializer.Serialize(body);
                var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}", data);

                if (response.IsSuccessStatusCode)
                {
                    string resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<FirebaseResult>(resultJson);

                    App.UserId = result.localId;

                    return true;
                }
                else
                {
                    string errorJson = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<Error>(errorJson);
                    MessageBox.Show(error.error.message);

                    return false;
                }
            }
        }

        public static async Task<bool> Login(User user)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var body = new
                    {
                        email = user.Username,
                        password = user.Password,
                        returnSecureToken = true
                    };

                    var bodyJson = JsonSerializer.Serialize(body);
                    var data = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={API_KEY}", data);

                    if (response.IsSuccessStatusCode)
                    {
                        string resultJson = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<FirebaseResult>(resultJson);

                        App.UserId = result.localId;

                        return true;
                    }
                    else
                    {
                        string errorJson = await response.Content.ReadAsStringAsync();
                        var error = JsonSerializer.Deserialize<Error>(errorJson);
                        MessageBox.Show(error.error.message);

                        return false;
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }
    }

    public class FirebaseResult
    {
        public string kind { get; set; }
        public string IdToken { get; set; }
        public string email { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
    }

    public class ErrorDetails
    {
        public int Code { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public ErrorDetails error { get; set; }
    }
}
