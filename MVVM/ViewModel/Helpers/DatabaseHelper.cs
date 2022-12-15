using Firebase.Storage;
using NotesApp.MVVM.Model;
using NotesApp.MVVM.View;
using NotesApp.MVVM.ViewModel.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace NotesApp.MVVM.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        private static string dbPath = "https://wpf-notes-app-c3ec4-default-rtdb.firebaseio.com/";

        public static readonly HttpClient httpClient = new HttpClient();
        public static string bucket = "wpf-notes-app-c3ec4.appspot.com";

        public static async Task<bool> Insert<T>(T item)
        {
            string jsonBody = JsonSerializer.Serialize(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


            var result = await httpClient.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static async Task<bool> Update<T>(T item) where T : IHasId
        {
            string jsonBody = JsonSerializer.Serialize(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var result = await httpClient.PatchAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);

            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public static async Task<bool> Delete<T>(T item) where T : IHasId
        {
            using (HttpClient client = new HttpClient())
            {

                if (item.GetType() == typeof(Note) && (item as Note).FileLocation != null)
                {
                    await new FirebaseStorage(bucket).Child((item as Note).Id + ".rtf").DeleteAsync();
                }

                var result = await httpClient.DeleteAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<List<T>> Read<T>() where T : IHasId
        {
            try
            {
                var result = await httpClient.GetAsync($"{dbPath}{typeof(T).Name.ToLower()}.json");
                var jsonResult = await result.Content.ReadAsStringAsync();

                if (jsonResult != "null" && result.IsSuccessStatusCode)
                {
                    var objects = JsonSerializer.Deserialize<Dictionary<string, T>>(jsonResult);

                    List<T> list = new List<T>();
                    foreach (var o in objects)
                    {
                        o.Value.Id = o.Key;
                        list.Add(o.Value);
                    }

                    return list;
                }
                else
                {
                    throw new Exception(result.ReasonPhrase);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            
        }

        public static async Task<string> UpdateFile(string filePath, string fileName)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                await new FirebaseStorage(bucket).Child(fileName).PutAsync(fileStream);
            }


            return $"{bucket}/{fileName}";
        }
    }
}
