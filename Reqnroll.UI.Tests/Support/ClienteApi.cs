using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Reqnroll.UI.Tests.Support
{
    internal sealed class ClienteApi
    {
        private readonly HttpClient _http;
        private readonly string _apiUrl;

        public ClienteApi(string apiUrl)
        {
            _apiUrl = apiUrl.TrimEnd('/');

            _http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        private sealed record ClienteDto(string id, string razon_Social, string nit, string email);

        public async Task DeleteByEmailExactAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            var url = $"{_apiUrl}/api/Cliente?email={Uri.EscapeDataString(email)}";

            HttpResponseMessage response;
            try
            {
                response = await _http.GetAsync(url);
            }
            catch
            {
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            List<ClienteDto>? clientes;
            try
            {
                clientes = await response.Content.ReadFromJsonAsync<List<ClienteDto>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return;
            }

            if (clientes == null || clientes.Count == 0)
            {
                return;
            }

            foreach (var c in clientes)
            {
                if (!string.Equals(c.email, email, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var deleteResp = await _http.DeleteAsync($"{_apiUrl}/api/Cliente/{Uri.EscapeDataString(c.id)}");
                    if (deleteResp.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
                    {
                        continue;
                    }
                }
                catch
                {
                    // no-op
                }
            }
        }

        public async Task DeleteIfExactMatchAsync(string razonSocial, string nit, string email)
        {
            if (string.IsNullOrWhiteSpace(razonSocial) || string.IsNullOrWhiteSpace(nit) || string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            // El endpoint filtra por Contains; usamos el query para acotar y luego matcheamos exacto aquí.
            var url = $"{_apiUrl}/api/Cliente?nit={Uri.EscapeDataString(nit)}";

            HttpResponseMessage response;
            try
            {
                response = await _http.GetAsync(url);
            }
            catch
            {
                // Si el API no está disponible, no bloqueamos el test de UI.
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            List<ClienteDto>? clientes;
            try
            {
                clientes = await response.Content.ReadFromJsonAsync<List<ClienteDto>>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return;
            }

            if (clientes == null || clientes.Count == 0)
            {
                return;
            }

            foreach (var c in clientes)
            {
                var razonMatch = string.Equals(c.razon_Social, razonSocial, StringComparison.OrdinalIgnoreCase);
                var nitMatch = string.Equals(c.nit, nit, StringComparison.OrdinalIgnoreCase);
                var emailMatch = string.Equals(c.email, email, StringComparison.OrdinalIgnoreCase);

                if (!razonMatch || !nitMatch || !emailMatch)
                {
                    continue;
                }

                try
                {
                    var deleteResp = await _http.DeleteAsync($"{_apiUrl}/api/Cliente/{Uri.EscapeDataString(c.id)}");
                    if (deleteResp.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound)
                    {
                        continue;
                    }
                }
                catch
                {
                    // no-op
                }
            }
        }
    }
}
