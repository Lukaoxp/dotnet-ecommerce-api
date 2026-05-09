using EcommerceApi.Domain.Exceptions;
using EcommerceApi.Domain.ValueObjects;

namespace EcommerceApi.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Money_WithSameAmountAndCurrency_AreEqual()
    {
        var a = Money.Of(100m, "BRL");
        var b = Money.Of(100m, "BRL");

        Assert.Equal(b, a);
    }

    [Fact]
    public void Money_WithDifferentCurrency_AreNotEqual()
    {
        var brl = Money.Of(100m, "BRL");
        var usd = Money.Of(100m, "USD");

        Assert.NotEqual(usd, brl);
    }

    [Fact]
    public void Add_WithSameCurrency_ReturnsSummedAmount()
    {
        var a = Money.Of(100m, "BRL");
        var b = Money.Of(50m, "BRL");

        var result = a.Add(b);

        Assert.Equal(Money.Of(150m, "BRL"), result);
    }

    [Fact]
    public void Add_WithDifferentCurrency_ThrowsDomainException()
    {
        var brl = Money.Of(100m, "BRL");
        var usd = Money.Of(50m, "USD");

        var ex = Assert.Throws<DomainException>(() => brl.Add(usd));
        Assert.Contains("currency", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Of_WithNonPositiveAmount_ThrowsDomainException(decimal amount)
    {
        Assert.Throws<DomainException>(() => Money.Of(amount, "BRL"));
    }
}