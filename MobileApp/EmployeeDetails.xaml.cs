using System.Collections.ObjectModel;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper.Configuration.Attributes;

namespace MobileApp;

public partial class EmployeeDetails : ContentPage
{
    public List<EmployeeInfo> employeeList = new();

    public string employeeID;

    public EmployeeDetails(params string[] data)
    {
        InitializeComponent();

        employeeID = data[0];

        EmployeeFirstName.Text = data[1];
        EmployeeLastName.Text = data[2];
        EmployeeDepartment.Text = data[3];
        EmployeeNumber.Text = data[4];
        EmployeeEmail.Text = data[5];
        EmployeeAddress1.Text = data[6];
        EmployeeAddress2.Text = data[7];
        EmployeePhoto.Source = data[8];
    }

    private async void OnUpdateButtonClicked(object sender, EventArgs e)
	{
        FileImageSource getCurrentImage = (FileImageSource)EmployeePhoto.Source;
        string employeePhoto = getCurrentImage.File;

        await Navigation.PushAsync(new UpdateDetails(
            employeeID,
            EmployeeFirstName.Text,
            EmployeeLastName.Text,
            EmployeeDepartment.Text,
            EmployeeNumber.Text,
            EmployeeAddress1.Text,
            EmployeeAddress2.Text,
            employeePhoto
            ));
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var userChoice = await DisplayActionSheet("⚠️ Alert\n\nAre you sure you want to delete this employee record?", "Cancel", "Yes, delete");

        if (userChoice == "Yes, delete")
        {
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = false
            };

            var localFile = "/INSERT/FILE/PATH/HERE";
            var tempFile = "/INSERT/FILE/PATH/HERE";

            using (Stream streamFile = await FileSystem.Current.OpenAppPackageFileAsync(localFile))
            using (var reader = new StreamReader(streamFile))
            using (var csv = new CsvReader(reader, config))
            using (var stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
            using (var csvWrite = new CsvWriter(writer, config))
            {
                while (csv.Read())
                {
                    employeeList.Add(new EmployeeInfo
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

                    foreach (var entry in employeeList)
                    {
                        if (entry.ID == Convert.ToInt32(employeeID))
                        {
                            employeeList.Remove(entry);
                            break;
                        }
                    }
                }

                csvWrite.WriteRecords(employeeList);
            }

            File.Delete(localFile);
            File.Move(tempFile, localFile);

            await DisplayAlert("Employee deleted", "You have successfully deleted the employee record", "OK");

            await Navigation.PushAsync(new MainPage());
        }
    }
}