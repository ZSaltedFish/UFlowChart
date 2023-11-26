using UnityEngine.Networking;

namespace Knight.UFlowChart.ChatGPT
{
    public class ChatGPTWebRequestCert : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //return base.ValidateCertificate(certificateData);
            return true;
        }
    }
}
