using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Xunit;

namespace Messaging.UnitTests;
/// <summary>
/// A test method attribute for injecting constructed objects into tests,
/// supplying its own specimens to overwrite AutoFixture's default behaviour.
/// </summary>
public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());

            return fixture;
        })
    {
    }
}

/// <summary>
/// A test method attribute for injecting constructed objects into tests,
/// supplying its own specimens to overwrite AutoFixture's default behaviour.
/// This attribute has the option of suppying in line parameters at a method level.
/// </summary>
public class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(
            new InlineDataAttribute(values),
            new AutoMoqDataAttribute())
    {
    }
}