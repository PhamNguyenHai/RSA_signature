using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;

namespace ChuKiDienTuRSA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnthoat1_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void btnthoat2_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void btnthoat3_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        // "Hàm kiểm tra hai số nguyên tố cùng nhau"
        public bool nguyenToCungNhau(int ai, int bi)
        {
            bool ktx_;
            // giải thuật Euclid;
            int temp;
            while (bi != 0)
            {
                temp = ai % bi;
                ai = bi;
                bi = temp;
            }
            if (ai == 1) { ktx_ = true; }
            else ktx_ = false;
            return ktx_;
        }

        //"Hàm kiểm tra nguyên tố"
        private bool RSA_kiemTraNguyenTo(int xi)
        {
            bool kiemtra = true;
            if (xi == 2 || xi == 3)
            {
                // kiemtra = true;
                return kiemtra;
            }
            else
            {
                if (xi == 1 || xi % 2 == 0 || xi % 3 == 0)
                {
                    kiemtra = false;
                }
                else
                {
                    for (int i = 5; i <= Math.Sqrt(xi); i = i + 6)
                        if (xi % i == 0 || xi % (i + 2) == 0)
                        {
                            kiemtra = false;
                            break;
                        }
                }
            }
            return kiemtra;
        }

        // Thuật toán bình phương và nhân
        public int RSA_mod(int mx, int ex, int nx)
        {

            //Chuyển e sang hệ nhị phân
            int[] a = new int[100];
            int k = 0;
            do
            {
                a[k] = ex % 2;
                k++;
                ex = ex / 2;
            }
            while (ex != 0);
            //Quá trình lấy dư
            int kq = 1;
            for (int i = k - 1; i >= 0; i--)
            {
                kq = (kq * kq) % nx;
                if (a[i] == 1)
                    kq = (kq * mx) % nx;
            }
            return kq;
        }

        //Thuật toán Euclide mở rộng tìm pt nghịch đảo
        public int EuclideMoRong(int a, int n) // a^-1 mod n
        {
            int r, q, y=0, y0 = 0, y1 = 1, tmp = n;
            while (a > 0)
            {
                r = n % a;
                q = n / a;
                if (r == 0) break;
                y = y0 - q * y1;
                y0 = y1;
                y1 = y;
                n = a;
                a = r;
            }
            if (a > 1)      return -1;          // GCD(a,n) # 1
            if (y >= 0)     return y;           //a^-1 mod n = y mod n
            else            return y + tmp;     //a^-1 mod n = -y mod n = y+n mod n
        }

        //Random ngau nhien
        private int RSA_ChonSoNgauNhien()
        {
            Random rd = new Random();
            return rd.Next(11, 101);// tốc độ chậm nên chọn số bé
        }

        int RSA_soP, RSA_soQ, RSA_soN, RSA_soE, RSA_soD, RSA_soPhi_n;   // E = B | D = A
        int F_rsa_d_dau = 0     /*Đánh dấu kiểm tra đã tạo khóa và ký chưa ?*/, check = 0;                                 //0 : Tai file | 1. Nhap tay
        string fileNameCanKi = "";
        string fileNameKiemTra = "";

        private void F_reset_()
        {
            Pvalue.Text = Qvalue.Text = Evalue.Text = NPubvalue.Text = Dvalue.Text = NPrvalue.Text = string.Empty;
        }
        private void F_TaoKhoa()
        {
            //Tinh n=p*q
            RSA_soN = RSA_soP * RSA_soQ;
            //Tính Phi(n)=(p-1)*(q-1)
            RSA_soPhi_n = (RSA_soP - 1) * (RSA_soQ - 1);
            //Tính e là một số ngẫu nhiên có giá trị 1< e <phi(n) và là số nguyên tố cùng nhau với Phi(n)
            do
            {
                Random RSA_rd = new Random();
                RSA_soE = RSA_rd.Next(2, RSA_soPhi_n);
            }
            while (!nguyenToCungNhau(RSA_soE, RSA_soPhi_n));
            //Tính d là nghịch đảo modulo của e
            RSA_soD = EuclideMoRong(RSA_soE, RSA_soPhi_n);
        }
        
        //Chuoi -> Unicode -> Binh phuong & nhan -> Chuoi ma hoa 
        public string F_MaHoa_RSA(string ChuoiVao1)
        {
            byte[] F_mh_temp1 = Encoding.Unicode.GetBytes(ChuoiVao1);       
            string F_base64 = Convert.ToBase64String(F_mh_temp1);
            // Chuyen xau thanh ma Unicode
            int[] F_mh_temp2 = new int[F_base64.Length];
            for (int i = 0; i < F_base64.Length; i++)
            {
                F_mh_temp2[i] = (int)F_base64[i];
            }

            //Mảng a chứa các kí tự đã mã hóa
            int[] F_mh_temp3 = new int[F_mh_temp2.Length];
            for (int i = 0; i < F_mh_temp2.Length; i++)
            {
                F_mh_temp3[i] = RSA_mod(F_mh_temp2[i], RSA_soD, RSA_soN); // mã hóa
            }

            //Chuyển sang kiểu kí tự trong bảng mã Unicode
            string Fmh_str = "";
            for (int i = 0; i < F_mh_temp3.Length; i++)
            {
                Fmh_str += (char)F_mh_temp3[i];
            }

            byte[] Fmh_data = Encoding.Unicode.GetBytes(Fmh_str);
            string F_ChuoiVBkemChuky = string.Empty;
            F_ChuoiVBkemChuky = Convert.ToBase64String(Fmh_data);
            return F_ChuoiVBkemChuky;
        }

        // hàm giải mã
        public string F_GiaiMa_RSA(string F_ChuoiVao2)
        {
            byte[] Fgm_temp1 = Convert.FromBase64String(F_ChuoiVao2);       //convert chuoi -> mang byte
            string Fgm_giaima = Encoding.Unicode.GetString(Fgm_temp1);

            int[] Fgm_temp2 = new int[Fgm_giaima.Length];
            for (int i = 0; i < Fgm_giaima.Length; i++)
            {
                Fgm_temp2[i] = (int)Fgm_giaima[i];
            }
            //Giải mã
            int[] Fgm_temp3 = new int[Fgm_temp2.Length];
            for (int i = 0; i < Fgm_temp3.Length; i++)
            {
                Fgm_temp3[i] = RSA_mod(Fgm_temp2[i], RSA_soE, RSA_soN);// giải mã
            }

            string Fgm_str2 = "";
            for (int i = 0; i < Fgm_temp3.Length; i++)
            {
                Fgm_str2 = Fgm_str2 + (char)Fgm_temp3[i];
            }
            byte[] F_gm_data2 = Convert.FromBase64String(Fgm_str2);

            string F_GM_ChuoiVBkemChuky = string.Empty;
            F_GM_ChuoiVBkemChuky = Encoding.Unicode.GetString(F_gm_data2);
            return F_GM_ChuoiVBkemChuky;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextHashFunc.IsEnabled = false;
            TextChuKiVaoFile.IsEnabled = false;
            Evalue.IsReadOnly = true;
            Dvalue.IsReadOnly = true;
            NPubvalue.IsReadOnly = true;
            NPrvalue.IsReadOnly = true;
        }

        // Đọc file word
        private string readFileContent(string path)
        {
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            object file = path;
            object nullobj = System.Reflection.Missing.Value;

            Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(
                ref file, ref nullobj, ref nullobj,
                ref nullobj, ref nullobj, ref nullobj,
                ref nullobj, ref nullobj, ref nullobj,
                ref nullobj, ref nullobj, ref nullobj);


            doc.ActiveWindow.Selection.WholeStory();
            doc.ActiveWindow.Selection.Copy();

            IDataObject data = Clipboard.GetDataObject();
            doc.Close(ref nullobj, ref nullobj, ref nullobj);
            wordApp.Quit(ref nullobj, ref nullobj, ref nullobj);
            return data.GetData(DataFormats.UnicodeText).ToString();
        }

        //Tự động tạo khóa
        private void btn_AutoInputKey_Click(object sender, RoutedEventArgs e)
        {
            F_reset_();
            RSA_soP = RSA_soQ = 0;
            do
            {
                RSA_soP = RSA_ChonSoNgauNhien();
                RSA_soQ = RSA_ChonSoNgauNhien();
            }
            while (RSA_soP == RSA_soQ || !RSA_kiemTraNguyenTo(RSA_soP) || !RSA_kiemTraNguyenTo(RSA_soQ));
            Pvalue.Text = RSA_soP.ToString();
            Qvalue.Text = RSA_soQ.ToString();
            F_TaoKhoa();
  
            Evalue.Text = RSA_soE.ToString();
            Dvalue.Text = RSA_soD.ToString();
            NPubvalue.Text = RSA_soN.ToString();
            NPrvalue.Text = RSA_soN.ToString();
            F_rsa_d_dau = 1;
            btn_AutoInputKey.IsEnabled = false;
            btnTaoKhoa.IsEnabled = false;
        }

        //Tạo khóa nhập vào ô
        private void btnTaoKhoa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Pvalue.Text == String.Empty || Qvalue.Text == String.Empty) throw new Exception("Nhập vào P, Q");
                RSA_soP = int.Parse(Pvalue.Text);
                RSA_soQ = int.Parse(Qvalue.Text);
                if (RSA_soP == RSA_soQ || !RSA_kiemTraNguyenTo(RSA_soP) || !RSA_kiemTraNguyenTo(RSA_soQ))
                    throw new Exception("P vs Q không hợp lệ !");
                F_TaoKhoa();
                
                Evalue.Text = RSA_soE.ToString();
                Dvalue.Text = RSA_soD.ToString();
                NPubvalue.Text = RSA_soN.ToString();
                NPrvalue.Text = RSA_soN.ToString();
                F_rsa_d_dau = 1;
                btn_AutoInputKey.IsEnabled = false;
                btnTaoKhoa.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Chọn file để ký
        private void btnChonFileKy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    fileNameCanKi = openFileDialog.FileName;
                    if(fileNameCanKi.Contains(".docx"))
                        TextNoiDungVBCanKi.Text = readFileContent(fileNameCanKi);
                    else
                    {
                        FileStream fsFileDauVao = new FileStream(openFileDialog.FileName, FileMode.Open);
                        StreamReader fr = new StreamReader(fsFileDauVao);
                        TextNoiDungVBCanKi.Text = fr.ReadToEnd();
                        fr.Close();
                    }
                    check = 0;
                    btnChuyen.IsEnabled = false;
                    TextNoiDungVBCanKi.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextNoiDungVBCanKi_changed(object sender, TextChangedEventArgs e)
        {
            check = 1;
        }

        //Thực hiện chuyển text khi nhập VB vào từ textbox
        private void btnChuyen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string TextCanKi = TextNoiDungVBCanKi.Text;
                if (TextCanKi == String.Empty)
                    throw new Exception("Không có nội dung để chuyển");
                else
                {
                    TextNoiDungVBCanKiemTra.Text = TextCanKi;
                    MessageBox.Show("Chuyển VB thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Thực hiện Ký
        private void btnKy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // thực hiên ký
                if (F_rsa_d_dau != 1)
                {
                    MessageBox.Show("Bạn chưa tạo chữ ký", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    byte mk = byte.Parse("111");
                    if (TextNoiDungVBCanKi.Text == string.Empty)
                    {
                        // MessageBox.Show("Phải nhập đủ 2 số ", "Thông Báo ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("Bạn chưa chọn file thực hiện ký!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MD5 md5 = MD5.Create();
                        byte[] FileVBKy_temp1;
                        if (check == 0)
                        {
                            FileStream fsFileDauVao = new FileStream(fileNameCanKi, FileMode.Open);
                            FileVBKy_temp1 = md5.ComputeHash(fsFileDauVao);
                            fsFileDauVao.Close();
                            fsFileDauVao.Dispose();
                        }
                        else
                        {
                            FileVBKy_temp1 = md5.ComputeHash(new UTF8Encoding().GetBytes(TextNoiDungVBCanKi.Text));
                        }
                        
                        //StringBuilder hash = new StringBuilder();
                        string hash = string.Empty;
                        for (int i = 0; i < FileVBKy_temp1.Length; i++)
                        {
                            //hash.Append(FileVBKy_temp1[i].ToString("x2"));
                            hash += FileVBKy_temp1[i].ToString("x2");
                        }
                        TextHashFunc.Text = hash.ToString();
                        string FileVBKy = Convert.ToBase64String(FileVBKy_temp1);
                        TextChuKiVaoFile.Text = F_MaHoa_RSA(FileVBKy);

                        F_rsa_d_dau = 2;
                        MessageBox.Show("Thực hiện ký thành công !", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLuuChuKi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(TextChuKiVaoFile.Text == string.Empty)
                    MessageBox.Show("Bạn chưa tạo chữ ký", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    if (saveFile.ShowDialog() == true)
                        File.WriteAllText(saveFile.FileName, TextChuKiVaoFile.Text);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnXacThuc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (F_rsa_d_dau == 2)
                {
                    if (TextNoiDungVBCanKiemTra.Text == string.Empty)
                        throw new Exception("Bạn chưa chọn Tài liệu kiểm tra chữ ký");
                    else if (TextChuKiXacNhan.Text == String.Empty)
                        throw new Exception("Bạn chưa tải chữ ký lên");
                    else if (EXacThuc.Text == String.Empty || NXacThuc.Text == string.Empty)
                        throw new Exception("Bạn chưa nhập đủ E & N");
                    else
                    {
                        if (int.Parse(EXacThuc.Text) != RSA_soE || int.Parse(NXacThuc.Text) != RSA_soN)
                            throw new Exception("E & N không chính xác");
                        else if (TextChuKiXacNhan.Text != TextChuKiVaoFile.Text)
                            throw new Exception("Chữ ký đã bị thay đổi");
                        else
                        {
                            MD5 md5 = MD5.Create();
                            byte[] FileVBKy_temp2;
                            if (check == 0)
                            {
                                FileStream fsFileDauVao = new FileStream(fileNameKiemTra, FileMode.Open);
                                FileVBKy_temp2 = md5.ComputeHash(fsFileDauVao);
                                fsFileDauVao.Close();
                                fsFileDauVao.Dispose();
                            }
                            else
                            {
                                FileVBKy_temp2 = md5.ComputeHash(new UTF8Encoding().GetBytes(TextNoiDungVBCanKiemTra.Text));
                            }
                            string ChuoiVBdiKem = Convert.ToBase64String(FileVBKy_temp2);
                            string VBKemChuKyGM = F_GiaiMa_RSA(TextChuKiXacNhan.Text); // thực hiện giải mã chữ ký
                                                                                        
                            int result = 0;
                            result = string.Compare(VBKemChuKyGM, ChuoiVBdiKem, true);

                            if (result == 0)
                            {
                                MessageBox.Show("Tài liệu gửi đến không bị chỉnh sửa gì", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Tài liệu gửi đến đã bị thay đổi", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Chưa thực hiện ký và gửi file ký", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTaiChuKiLen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    FileStream FileTaiChuKi = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader fr = new StreamReader(FileTaiChuKi);
                    TextChuKiXacNhan.Text = fr.ReadToEnd();
                    fr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnChonFileXacThuc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    fileNameKiemTra = openFileDialog.FileName;
                    if (fileNameKiemTra.Contains(".docx"))
                        TextNoiDungVBCanKiemTra.Text = readFileContent(fileNameKiemTra);
                    else
                    {
                        FileStream fsFileKiemTra = new FileStream(openFileDialog.FileName, FileMode.Open);
                        StreamReader fr = new StreamReader(fsFileKiemTra);
                        TextNoiDungVBCanKiemTra.Text = fr.ReadToEnd();
                        fr.Close();
                    }
                    TextNoiDungVBCanKiemTra.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}