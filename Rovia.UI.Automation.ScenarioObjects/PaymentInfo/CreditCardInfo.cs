

using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class CreditCardInfo
    {
        public CreditCardType CardType { get; private set; }
        public string CardNo { get; private set; }
        public string Cvv { get; private set; }
        public string NameOnCard { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public string CardHolderEmailId { get; private set; }

        private static readonly string[] CardNumbers =
        {
            "4111111111111111",
            //"5424000000000015",
            //"370000000000002",
            //"6011000000000012",
            //"3528288605211810"
        };

        public CreditCardInfo(CreditCardType cardType)
        {
            CardType = cardType;
            CardNo = CardNumbers[(int) cardType];
            Cvv = cardType == CreditCardType.AmericanExpress ? "9999" : "999";
            NameOnCard = "UIAutomationTestAccount";
            ExpiryDate = DateTime.Now.AddYears(10);
            CardHolderEmailId = "rpandit@tavisca.com";
        }
    }
}

