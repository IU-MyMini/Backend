using GradingModule.Application.Commands;

namespace GradingModule.Tests.UnitTests;

public partial class GradingTest
{
    [Fact]
    public void Sum()
    {
        // Arrange
        var a = 5;
        var b = 10;

        // Act
        var c = a + b;

        // Assert 
        Assert.Equal(15, c);
    }

    [Fact]
    public async void Hello()
    {
        var commandHandler = new HelloCommandHandler();
        var result = await commandHandler.Handle(new HelloCommand(), CancellationToken.None);
        Assert.Equal("Hello!", result);
    }
}