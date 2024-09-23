using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestPayment
{
    public class Program
    {
        private readonly HttpClient _httpClient;

        public Program()
        {
            _httpClient = new HttpClient();
        }

        // Main method serves as the entry point
        public static async Task Main(string[] args)
        {
            var program = new Program();

            // Example data (replace with actual values)
            string cardNumber = "4111111111111111";  // Test credit card number
            string expiryYear = "25";
            string expiryMonth = "12";
            string firstName = "Achyut";
            string lastName = "Gaihre";
            decimal amount = 100.50m;  // Payment amount
            string orderCode = "ORD123456";  // Unique order code

            try
            {
                // Generate token
                string token = await program.GenerateTokenAsync(cardNumber, expiryYear, expiryMonth, firstName, lastName);
                Console.WriteLine($"Generated Token: {token}");

                // Process payment
                string paymentResponse = await program.ProcessPaymentAsync(token, amount, orderCode);
                Console.WriteLine($"Payment Response: {paymentResponse}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Method to create a token from card details
        public async Task<string> GenerateTokenAsync(string cardNumber, string expiryYear, string expiryMonth, string firstName, string lastName)
        {
            var tokenEndpoint = "https://credit.j-payment.co.jp/gateway/gateway_token.aspx";

            var values = new Dictionary<string, string>
        {
            { "aid", "128875" },  // Replace with actual Store ID
            { "cn", cardNumber },
            { "ed", $"{expiryYear}{expiryMonth}" },
            { "fn", firstName },
            { "ln", lastName }
        };

            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Parse and return token from response
            return ExtractToken(responseString);
        }

        // Method to process payment using the generated token
        public async Task<string> ProcessPaymentAsync(string token, decimal amount, string orderCode)
        {
            var paymentEndpoint = "https://credit.j-payment.co.jp/gateway/gateway.aspx";

            var values = new Dictionary<string, string>
        {
            { "aid", "128875" },  // Replace with actual Store ID
            { "tkn", token },
            { "am", amount.ToString() },
            { "jb", "AUTH" },  // 'AUTH' for hold, 'CAPTURE' for direct charge
            { "cod", orderCode }
        };

            var content = new FormUrlEncodedContent(values);
            var response = await _httpClient.PostAsync(paymentEndpoint, content);
            return await response.Content.ReadAsStringAsync();
        }

        private string ExtractToken(string response)
        {
            // Logic to extract the token from the response
            return "parsed_token";  // Modify based on the actual API response
        }
    }
}
