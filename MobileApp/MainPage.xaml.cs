using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace MobileApp;

public partial class MainPage : ContentPage
{
	ObservableCollection<EmployeeInfo> employees = new();
    public ObservableCollection<EmployeeInfo> Employees { get { return employees; } }

    public MainPage()
	{
		InitializeComponent();

        GetEmployees();
    }

    private async void OnEmployeeSelected(object sender, ItemTappedEventArgs e)
	{
        var employeeSelected = e.Item as EmployeeInfo;

        var firstName = employeeSelected.FirstName;
        var lastName = employeeSelected.LastName;

        await Navigation.PushAsync(new EmployeeDetails(
            employeeSelected.ID.ToString(),
            firstName,
            lastName,
            employeeSelected.Department,
            employeeSelected.PhoneNumber,
            (firstName + "." + lastName + "@roi.com.au").ToLower(),
            employeeSelected.AddressLine1,
            employeeSelected.AddressLine2,
            employeeSelected.ProfilePic
            ));
    }

    public async void GetEmployees()
    {
        EmployeeView.RowHeight = 150;
        EmployeeView.ItemsSource = employees;

        CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = false
        };

        var localFile = "/INSERT/FILE/PATH/HERE";

        if (!File.Exists(localFile))
        {
            HttpClient server = new HttpClient();
            server.BaseAddress = new Uri("http://localhost/");
            HttpResponseMessage response = await server.GetAsync("INSERTFILENAMEHERE.csv");

            var serverFile = await response.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(serverFile))
            using (var csv = new CsvReader(reader, config))
            using (var stream = new FileStream(localFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            using (var csvWrite = new CsvWriter(writer, config))
            {
                while (csv.Read())
                {
                    employees.Add(new EmployeeInfo
                    {
                        ID = csv.GetField<int>(0),
                        FirstName = csv.GetField<string>(1),
                        LastName = csv.GetField<string>(2),
                        Department = csv.GetField<string>(3),
                        PhoneNumber = csv.GetField<string>(4),
                        AddressLine1 = csv.GetField<string>(5),
                        AddressLine2 = csv.GetField<string>(6),
                        ProfilePic = csv.GetField<string>(7)
                    });
                }

                csvWrite.WriteRecords(employees);
            }
        }

        else
        {
            using (Stream stream = await FileSystem.Current.OpenAppPackageFileAsync(localFile))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    employees.Add(new EmployeeInfo
                    {
                        ID = csv.GetField<int>(0),
                        FirstName = csv.GetField<string>(1),
                        LastName = csv.GetField<string>(2),
                        Department = csv.GetField<string>(3),
                        PhoneNumber = csv.GetField<string>(4),
                        AddressLine1 = csv.GetField<string>(5),
                        AddressLine2 = csv.GetField<string>(6),
                        ProfilePic = csv.GetField<string>(7)
                    });
                }
            }
        }
    }
}

public class EmployeeInfo
{
    public int ID { get; set; }
	public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Department { get; set; }

    public string PhoneNumber { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }

    public string ProfilePic { get; set; }
}