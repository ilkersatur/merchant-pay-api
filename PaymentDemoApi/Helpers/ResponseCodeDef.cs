namespace VposApi.Helpers
{
    public static class ResponseCodeDef
    {
        public static readonly Dictionary<string, string> ResponseCodeDefList = new Dictionary<string, string>
    {
        { "0000", "Onaylandı" },
        { "0001", "Bankanızı Arayınız" },
        { "0002", "Bankanızı Arayınız" },
        { "0003", "Bankanızı Arayınız" },
        { "0004", "Kapalı Kart" },
        { "0005", "Karta İzin Verilmeyen İşlem" },
        { "0006", "Provizyon Alınamadı" },
        { "0007", "Provizyon Alınamadı Kart Hatası" },
        { "0008", "Kimlik Kontrolü" },
        { "0009", "Tekrar Deneyin / Kartı Takın" },
        { "0012", "Geçersiz İşlem" },
        { "0013", "Tutar Hatalı" },
        { "0014", "Geçersiz Kart Numarası" },
        { "0015", "Geçersiz Issuer Banka" },
        { "0025", "Could not connect to the Merchant Payment Server" },
        { "0028", "Provizyon Alınamadı" },
        { "0029", "Orijinal İşlem Bulunamadı" },
        { "0030", "Format Hatası" },
        { "0033", "Vadesi Dolmuş Kart" },
        { "0034", "Karta El Koyunuz" },
        { "0036", "Kısıtlı Kart" },
        { "0038", "İzin Verilen PIN Denemeleri Aşıldı" },
        { "0039", "Desteklenmeyen Mesaj Güvenlik Kodu" },
        { "0041", "Kayıp Kart" },
        { "0043", "Çalıntı Kart" },
        { "0046", "Kapalı Hesap" },
        { "0051", "Limit Yetersiz" },
        { "0052", "Hesap Numarasını Kontrol Edin" },
        { "0053", "Döviz Hesabı Bulunamadı" },
        { "0054", "Vadesi Dolmuş Kart" },
        { "0055", "Şifre Hatalı" },
        { "0057", "Kartın Desteklemediği İşlem" },
        { "0058", "Terminale izin verilmeyen İşlem" },
        { "0059", "Şüpheli Dolandırıcılık" },
        { "0061", "Nakit Çekim Limiti Aşımı" },
        { "0062", "Kısıtlanmış Kart" },
        { "0063", "Güvenlik İhlali" },
        { "0065", "Nakit Çekim İşlem Sayısı Aşımı" },
        { "0070", "Kart Bankasıyla İletişime Geçin" },
        { "0071", "PIN Değişmedi" },
        { "0075", "Hatalı Şifre Deneme Sayısı Aşıldı" },
        { "0076", "KEY Doğrulanamadı" },
        { "0077", "Komut Dosyası Yok" },
        { "0078", "Geçersiz/Var Olmayan Hesap" },
        { "0079", "ARQC Hatalı" },
        { "0082", "Hatalı CVV" },
        { "0085", "Onaylandı" },
        { "0086", "PIN Doğrulanamıyor" },
        { "0087", "Anahtar Eşitleme Hatası" },
        { "0088", "Şifreleme Hatası" },
        { "0089", "PIN Kabul Edilemez" },
        { "0091", "Cevap Alınamadı" },
        { "0092", "Sistem Hatası" },
        { "0093", "Kanun İhlali - İşlem Tamamlanamıyor" },
        { "0095", "Sistem Hatası" },
        { "0096", "Sistem Hatası" },
        { "0098", "Yinelenen Mesaj" },
        { "0100", "Batch Numarası Hatalı" },
        { "0999", "Genel Hata" },
        { "N",    "Bankanızı Arayınız" } // Özel kayıt gibi görünüyor
    };

        public static string GetResponseCodeDefList(string code)
        {
            return ResponseCodeDefList.TryGetValue(code, out var message) ? message : "Bilinmeyen Hata";
        }
    }

}
