using System.Text.Json.Serialization;

namespace PromoLimit.Models
{
// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class AlternativePhone
    {
        [JsonPropertyName("extension")]
        public string Extension { get; set; }

        [JsonPropertyName("area_code")]
        public string AreaCode { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }
    }

    public class AtmTransferReference
    {
        [JsonPropertyName("company_id")]
        public object CompanyId { get; set; }

        [JsonPropertyName("transaction_id")]
        public object TransactionId { get; set; }
    }

    public class Buyer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }

    public class Collector
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class Context
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("site")]
        public string Site { get; set; }

        [JsonPropertyName("flows")]
        public List<object> Flows { get; set; }
    }

    public class Coupon
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }

    public class Feedback
    {
        [JsonPropertyName("buyer")]
        public object Buyer { get; set; }

        [JsonPropertyName("seller")]
        public object Seller { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("variation_id")]
        public long? VariationId { get; set; }

        [JsonPropertyName("seller_custom_field")]
        public string SellerCustomField { get; set; }

        [JsonPropertyName("variation_attributes")]
        public List<VariationAttribute> VariationAttributes { get; set; }

        [JsonPropertyName("warranty")]
        public object Warranty { get; set; }

        [JsonPropertyName("condition")]
        public string Condition { get; set; }

        [JsonPropertyName("seller_sku")]
        public string SellerSku { get; set; }

        [JsonPropertyName("global_price")]
        public object GlobalPrice { get; set; }

        [JsonPropertyName("net_weight")]
        public object NetWeight { get; set; }
    }

    public class OrderItem
    {
        [JsonPropertyName("item")]
        public Item Item { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("requested_quantity")]
        public RequestedQuantity RequestedQuantity { get; set; }

        [JsonPropertyName("picked_quantity")]
        public object PickedQuantity { get; set; }

        [JsonPropertyName("unit_price")]
        public double UnitPrice { get; set; }

        [JsonPropertyName("full_unit_price")]
        public double FullUnitPrice { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; }

        [JsonPropertyName("manufacturing_days")]
        public object ManufacturingDays { get; set; }

        [JsonPropertyName("sale_fee")]
        public double SaleFee { get; set; }

        [JsonPropertyName("listing_type_id")]
        public string ListingTypeId { get; set; }
    }

    public class OrderRequest
    {
        [JsonPropertyName("return")]
        public object Return { get; set; }

        [JsonPropertyName("change")]
        public object Change { get; set; }
    }

    public class Payment
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("order_id")]
        public long OrderId { get; set; }

        [JsonPropertyName("payer_id")]
        public int PayerId { get; set; }

        [JsonPropertyName("collector")]
        public Collector Collector { get; set; }

        [JsonPropertyName("card_id")]
        public object CardId { get; set; }

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("payment_method_id")]
        public string PaymentMethodId { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; }

        [JsonPropertyName("installments")]
        public int Installments { get; set; }

        [JsonPropertyName("issuer_id")]
        public object IssuerId { get; set; }

        [JsonPropertyName("atm_transfer_reference")]
        public AtmTransferReference AtmTransferReference { get; set; }

        [JsonPropertyName("coupon_id")]
        public object CouponId { get; set; }

        [JsonPropertyName("activation_uri")]
        public object ActivationUri { get; set; }

        [JsonPropertyName("operation_type")]
        public string OperationType { get; set; }

        [JsonPropertyName("payment_type")]
        public string PaymentType { get; set; }

        [JsonPropertyName("available_actions")]
        public List<string> AvailableActions { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("status_code")]
        public object StatusCode { get; set; }

        [JsonPropertyName("status_detail")]
        public string StatusDetail { get; set; }

        [JsonPropertyName("transaction_amount")]
        public double TransactionAmount { get; set; }

        [JsonPropertyName("transaction_amount_refunded")]
        public int TransactionAmountRefunded { get; set; }

        [JsonPropertyName("taxes_amount")]
        public int TaxesAmount { get; set; }

        [JsonPropertyName("shipping_cost")]
        public int ShippingCost { get; set; }

        [JsonPropertyName("coupon_amount")]
        public int CouponAmount { get; set; }

        [JsonPropertyName("overpaid_amount")]
        public int OverpaidAmount { get; set; }

        [JsonPropertyName("total_paid_amount")]
        public double TotalPaidAmount { get; set; }

        [JsonPropertyName("installment_amount")]
        public object InstallmentAmount { get; set; }

        [JsonPropertyName("deferred_period")]
        public object DeferredPeriod { get; set; }

        [JsonPropertyName("date_approved")]
        public DateTime DateApproved { get; set; }

        [JsonPropertyName("authorization_code")]
        public object AuthorizationCode { get; set; }

        [JsonPropertyName("transaction_order_id")]
        public object TransactionOrderId { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("date_last_modified")]
        public DateTime DateLastModified { get; set; }
    }

    public class Phone
    {
        [JsonPropertyName("extension")]
        public string Extension { get; set; }

        [JsonPropertyName("area_code")]
        public object AreaCode { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
    }

    public class RequestedQuantity
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("measure")]
        public string Measure { get; set; }
    }

    public class MlOrder
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("date_closed")]
        public DateTime DateClosed { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("manufacturing_ending_date")]
        public object ManufacturingEndingDate { get; set; }

        [JsonPropertyName("comment")]
        public object Comment { get; set; }

        [JsonPropertyName("pack_id")]
        public long PackId { get; set; }

        [JsonPropertyName("pickup_id")]
        public object PickupId { get; set; }

        [JsonPropertyName("order_request")]
        public OrderRequest OrderRequest { get; set; }

        [JsonPropertyName("fulfilled")]
        public bool Fulfilled { get; set; }

        [JsonPropertyName("mediations")]
        public List<object> Mediations { get; set; }

        [JsonPropertyName("total_amount")]
        public double TotalAmount { get; set; }

        [JsonPropertyName("paid_amount")]
        public double PaidAmount { get; set; }

        [JsonPropertyName("coupon")]
        public Coupon Coupon { get; set; }

        [JsonPropertyName("expiration_date")]
        public DateTime ExpirationDate { get; set; }

        [JsonPropertyName("order_items")]
        public List<OrderItem> OrderItems { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; }

        [JsonPropertyName("payments")]
        public List<Payment> Payments { get; set; }

        [JsonPropertyName("shipping")]
        public Shipping Shipping { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("status_detail")]
        public object StatusDetail { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("feedback")]
        public Feedback Feedback { get; set; }

        [JsonPropertyName("context")]
        public Context Context { get; set; }

        [JsonPropertyName("buyer")]
        public Buyer Buyer { get; set; }

        [JsonPropertyName("seller")]
        public Seller Seller { get; set; }

        [JsonPropertyName("taxes")]
        public Taxes Taxes { get; set; }
    }

    public class Seller
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("phone")]
        public Phone Phone { get; set; }

        [JsonPropertyName("alternative_phone")]
        public AlternativePhone AlternativePhone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class Shipping
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }

    public class Taxes
    {
        [JsonPropertyName("amount")]
        public object Amount { get; set; }

        [JsonPropertyName("currency_id")]
        public object CurrencyId { get; set; }

        [JsonPropertyName("id")]
        public object Id { get; set; }
    }

    public class VariationAttribute
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value_id")]
        public object ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string ValueName { get; set; }
    }


}
