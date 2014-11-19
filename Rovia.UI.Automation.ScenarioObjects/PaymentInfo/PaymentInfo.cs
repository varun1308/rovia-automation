

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class PaymentInfo
    {
        public PaymentMode PaymentMode { get; private set; }
        public CreditCardInfo CreditCardInfo { get; private set; }
        public BillingAddress BillingAddress { get; private set; }

        public PaymentInfo(PaymentMode paymentMode):this(paymentMode,CreditCardType.Visa)
        {
            
        }

        public PaymentInfo(PaymentMode paymentMode, CreditCardType cardType)
        {
            PaymentMode = paymentMode;
            BillingAddress=new BillingAddress();
            CreditCardInfo = paymentMode == PaymentMode.CreditCard ? new CreditCardInfo(cardType):null;            
        }

    }
}
