
public class VendorData
{
    public VendorData(IItemContainer sellingItemContainer)
    {
        SellingItemContainer = sellingItemContainer;
    }

    public IItemContainer SellingItemContainer { get; } = null;
}
