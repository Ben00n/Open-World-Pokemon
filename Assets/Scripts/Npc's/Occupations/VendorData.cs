
public class VendorData
{
    public VendorData(IItemContainer sellingItemContainer, IItemContainer buyingItemContainer)
    {
        SellingItemContainer = sellingItemContainer;
        BuyingItemContainer = buyingItemContainer;
    }

    public IItemContainer SellingItemContainer { get; } = null;
    public IItemContainer BuyingItemContainer { get; } = null;
}
