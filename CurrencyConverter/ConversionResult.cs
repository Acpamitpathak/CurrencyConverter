public class ProductManagementSteps
{
    private readonly HttpClient _client;
    private Product _newProduct;
    private HttpResponseMessage _response;

    // Your actual connection string (replace with your actual values)
    private readonly string _connectionString = "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;";

    public ProductManagementSteps()
    {
        // Configure WebApplicationFactory to use your actual database connection string
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext configuration (in-memory or development DbContext)
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                    services.Remove(descriptor);

                    // Add DbContext configured to use the real database with the provided connection string
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlServer(_connectionString);
                    });
                });
            });

        // Create HttpClient for API interaction
        _client = factory.CreateClient();
    }

    // SpecFlow step definitions
    [Given(@"I have a new product with name ""(.*)"" and price ""(.*)""")]
    public void GivenIHaveANewProductWithNameAndPrice(string name, decimal price)
    {
        _newProduct = new Product { Name = name, Price = price };
    }

    [When(@"I send a POST request to add the product")]
    public async Task WhenISendAPOSTRequestToAddTheProduct()
    {
        _response = await _client.PostAsJsonAsync("/api/products", _newProduct);
    }

    [Then(@"the product should be added successfully")]
    public void ThenTheProductShouldBeAddedSuccessfully()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
