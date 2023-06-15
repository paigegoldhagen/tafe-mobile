using System.Drawing.Imaging;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MobileApp;

public partial class SettingsPage : ContentPage
{
    public List<SettingsInfo> currentSettings = new();
    public List<SettingsInfo> settings = new();

    public bool currentTheme;
    public string currentTextSize;
    public string currentUsername;

    public string filePath = "settings.csv";

    public CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ",",
        HasHeaderRecord = false
    };

    public SettingsPage()
	{
		InitializeComponent();

        LoadSettings();
    }

    // Get settings from file, change appearance based on variables
    public async void LoadSettings()
    {
        using (Stream stream = await FileSystem.Current.OpenAppPackageFileAsync(filePath))
        using (var reader = new StreamReader(stream))
        using (var csv = new CsvReader(reader, config))
        {
            while (csv.Read())
            {
                currentTheme = csv.GetField<bool>(0);
                currentTextSize = csv.GetField<string>(1);
                currentUsername = csv.GetField<string>(2);

                TextInput.Text = currentUsername;

                if (currentTheme)
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    ThemeSwitch.IsToggled = true;
                }
                else
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                    ThemeSwitch.IsToggled = false;
                }

                switch (currentTextSize)
                {
                    case "Small":
                        SetSmallText();
                        break;

                    case "Large":
                        SetLargeText();
                        break;
                }
            }
        }
    }

	public void OnSmallTextSelected(object sender, EventArgs e)
	{
        currentTextSize = "Small";

        SetSmallText();
    }

    public void OnLargeTextSelected(object sender, EventArgs e)
    {
        currentTextSize = "Large";

        SetLargeText();
    }

    public void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        bool isDarkTheme = e.Value;
        Preferences.Set("DarkThemeOn", isDarkTheme ? "Dark" : "Light");

        if (isDarkTheme)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }

    // Get current settings from toggle, buttons, user input and save to file
    public async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        currentSettings.Add(new SettingsInfo
        {
            DarkTheme = ThemeSwitch.IsToggled,
            TextSize = currentTextSize,
            Username = TextInput.Text
        });

        var tempFile = "/FILE/PATH/HERE/settings_temp.csv";

        using (var stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        using (var writer = new StreamWriter(stream))
        using (var csvWrite = new CsvWriter(writer, config))
        {
            foreach (var entry in currentSettings.ToList())
            {
                csvWrite.WriteRecord(entry);
                currentSettings.Remove(entry);
            }
        }

        File.Delete(filePath);
        File.Move(tempFile, filePath);

        await DisplayAlert("Settings saved", "The user settings have been updated", "OK");
    }

    public void SetSmallText()
    {
        LargeTextButton.IsEnabled = true;

        HeaderText.FontSize -= 5;
        ModeTitle.FontSize -= 5;
        SizeTitle.FontSize -= 5;
        UsernameTitle.FontSize -= 5;
        TextInput.FontSize -= 5;
        ButtonLabel.FontSize -= 5;

        SmallTextButton.IsEnabled = false;
    }

    public void SetLargeText()
    {
        SmallTextButton.IsEnabled = true;

        HeaderText.FontSize += 5;
        ModeTitle.FontSize += 5;
        SizeTitle.FontSize += 5;
        UsernameTitle.FontSize += 5;
        TextInput.FontSize += 5;
        ButtonLabel.FontSize += 5;

        LargeTextButton.IsEnabled = false;
    }
}

public class SettingsInfo
{
	public bool DarkTheme { get; set; }
    public string TextSize { get; set; }
    public string Username { get; set; }
}