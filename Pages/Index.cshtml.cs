using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace rts_example.Pages
{
    public class IndexModel : PageModel
    {
        public string SandboxGatewayUrl = "https://cert.api2.heartlandportico.com/hps.exchange.posgateway/posgatewayservice.asmx";
        public string SandboxSecretApiKey = "skapi_cert_MTyMAQBiHVEAewvIzXVFcmUd2UcyBge_eCpaASUp0A";

        public string GatewayResponse { get; set; }

        public void OnGet()
        {

        }

        public async Task OnPost()
        {
            var token = Request.Form["paymentReference"];
            var zip = Request.Form["billingZip"];

            var client = new HttpClient();
            var content = new StringContent(
                GetCNPXMLForTokenAndZip(SandboxSecretApiKey, "10.00", token[0], zip[0]),
                Encoding.UTF8,
                "text/xml"
            );
            var response = await client.PostAsync(SandboxGatewayUrl, content);
            GatewayResponse = await response.Content.ReadAsStringAsync();
        }

    private string GetCNPXMLForTokenAndZip(string apiKey, string amount, string token, string zip)
    {
      return @"<?xml version=""1.0"" encoding=""utf-8""?>
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:hps=""http://Hps.Exchange.PosGateway"">
	<soapenv:Body>
		<hps:PosRequest>
			<hps:Ver1.0>
				<hps:Header>
					<hps:SecretAPIKey>" + apiKey + @"</hps:SecretAPIKey>
				</hps:Header>
				<hps:Transaction>
					<hps:CreditSale>
						<hps:Block1>
							<hps:AllowDup>Y</hps:AllowDup>
							<hps:Amt>" + amount + @"</hps:Amt>
							<hps:CardHolderData>
								<hps:CardHolderZip>" + zip + @"</hps:CardHolderZip>
							</hps:CardHolderData>
							<hps:CardData>
								<hps:TokenData>
                                    <hps:TokenValue>" + token + @"</hps:TokenValue>
                                </hps:TokenData>
							</hps:CardData>
						</hps:Block1>
					</hps:CreditSale>
				</hps:Transaction>
			</hps:Ver1.0>
		</hps:PosRequest>
	</soapenv:Body>
</soapenv:Envelope>";
    }
  }
}
