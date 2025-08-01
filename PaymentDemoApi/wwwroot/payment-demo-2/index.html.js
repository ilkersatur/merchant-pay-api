document.addEventListener('DOMContentLoaded', () => {
    // Mevcut buton olayları
    const createOrderBtn = document.getElementById('createOrderBtn');
    if (createOrderBtn) {
        createOrderBtn.addEventListener('click', async () => {
            const requiredFields = [];

            // Ortak alanlar
            const merchantNumber = document.getElementById("merchantNumber");
            const shopCode = document.getElementById("shopCode");
            const userId = document.getElementById("userId");
            const password = document.getElementById("password");

            // İşlem bilgileri
            const transactionType = document.getElementById("transactionType").value.toUpperCase();
            const transactionId = document.getElementById("transactionId");
            const amount = document.getElementById("amount");
            const currency = document.getElementById("currency");
            const bin = document.getElementById("bin");
            const lastFourDigits = document.getElementById("lastFourDigits");
            const tcknVkn = document.getElementById("tcknVkn");

            // Temel zorunlu alanlar
            requiredFields.push(merchantNumber, shopCode, userId, password);

            // İşlem türüne göre ek zorunlu alanlar
            switch (transactionType) {
                case "SATIŞ":
                case "MAIL ORDER":
                case "ÖN PROVIZYON":
                    requiredFields.push(amount, currency);
                    break;

                case "ÖN PROVIZYON KAPAMA":
                    requiredFields.push(transactionId, amount);
                    break;

                case "İPTAL":
                    requiredFields.push(transactionId);
                    break;

                case "İADE":
                    requiredFields.push(transactionId, amount);
                    break;

                case "MOTO INSURANCE":
                    requiredFields.push(bin, lastFourDigits, tcknVkn, currency, amount);
                    break;
            }

            // Validation kontrolü
            let isValid = true;
            requiredFields.forEach(input => {
                if (!input || input.value.trim() === "") {
                    input.classList.add("input-error");
                    isValid = false;
                } else {
                    input.classList.remove("input-error");
                }
            });

            if (isValid) {

                const mapTransactionType = (type) => {
                    switch (type) {
                        case "ÖN PROVIZYON KAPAMA":
                            return "POSTAUTH";
                        case "İPTAL":
                            return "VOID";
                        case "İADE":
                            return "REFUND";
                        case "MOTO INSURANCE":
                            return "MOTOINSURANCE";
                        default:
                            return type;
                    }
                };

                const orderData = {
                    merchantNumber: document.getElementById('merchantNumber').value,
                    shopCode: document.getElementById('shopCode').value,
                    userId: document.getElementById('userId').value,
                    password: document.getElementById('password').value,
                    transactionType: document.getElementById('transactionType').value,
                    currency: document.getElementById('currency').value,
                    amount: document.getElementById('amount').value,
                    installment: document.getElementById('installment').value,
                    transactionId: document.getElementById('transactionId').value,
                    bin: document.getElementById('bin').value,
                    lastFourDigits: document.getElementById('lastFourDigits').value,
                    tcknVkn: document.getElementById('tcknVkn').value,
                    storeType: document.getElementById('storeType').value,
                    secureTransactionType: document.getElementById('secureTransactionType').value
                };

                if (orderData.transactionType === "SATIŞ" || orderData.transactionType === "ÖN PROVIZYON" || orderData.transactionType === "MAIL ORDER") {

                    let secure = document.querySelector('.dropdown.active-dropdown').querySelector('.dropbtn').textContent.trim()

                    localStorage.clear();

                    if (secure === 'NonSecure') {
                        localStorage.setItem('orderFormData', JSON.stringify(orderData));
                        window.location.href = 'payment.html';
                    }
                    else if (secure === 'Secure') {
                        localStorage.setItem('secureOrderFormData', JSON.stringify(orderData));
                        if (orderData.storeType === "3D PAY HOSTING") {
                            window.location.href = 'hosting.html';
                        } else {
                            window.location.href = 'payment.html';
                        }
                    }

                } else if (orderData.transactionType === "İPTAL" || orderData.transactionType === "İADE" || orderData.transactionType === "MOTO INSURANCE" || orderData.transactionType === "ÖN PROVIZYON KAPAMA") {
                    const requestData = {
                        userId: document.getElementById('userId').value,
                        password: document.getElementById('password').value,
                        merchantNumber: document.getElementById('merchantNumber').value,
                        shopCode: document.getElementById('shopCode').value,
                        transactionType: mapTransactionType(document.getElementById('transactionType').value),
                        transactionAmount: (() => {
                            const rawAmount = (orderData ? orderData.amount : secureOrderData.amount)?.replace(',', '.');
                            const parsedAmount = parseFloat(rawAmount);
                            const amountInCents = Math.round(parsedAmount * 100);
                            return isNaN(amountInCents) ? null : String(amountInCents);
                        })(),
                        currencyCode: document.getElementById('currency').value == "TRY" ? "949" : "NaN",
                        installmentCount: document.getElementById('installment').value,
                        securityType: "NonSecure",
                        rewardAmount: "0",
                        transactionId: document.getElementById('transactionId').value,
                        bin: document.getElementById('bin').value,
                        lastFourDigits: document.getElementById('lastFourDigits').value,
                        tcknVkn: document.getElementById('tcknVkn').value
                    };

                    document.getElementById("loading").style.display = "block";

                    try {
                        const response = await fetch("https://localhost:7186/api/Payment", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json"
                            },
                            body: JSON.stringify(requestData)
                        });

                        const result = await response.json();
                        console.log("Yanıt:", result);

                        localStorage.clear();

                        localStorage.setItem('orderFormData', JSON.stringify(orderData));
                        localStorage.setItem("paymentResult", JSON.stringify(result));

                        window.location.href = "result.html";

                        // alert("İşlem sonucu: " + (result.success ? "Başarılı" : "Hata"));
                    } catch (error) {
                        console.error("İstek hatası:", error);
                        alert("İstek gönderilirken hata oluştu.");
                    }
                    finally {
                        document.getElementById("loading").style.display = "none"; // Gizle
                    }
                }
            }

        });
    }

    // --- Dropdown menü için JavaScript (isteğe bağlı, mobil uyumluluk için) ---
    const dropdowns = document.querySelectorAll('.dropdown');

    dropdowns.forEach(dropdown => {
        const dropbtn = dropdown.querySelector('.dropbtn');
        dropbtn.addEventListener('click', function (event) {
            event.preventDefault(); // Varsayılan bağlantı davranışını engelle
            dropdown.classList.toggle('active'); // "active" sınıfını ekle/kaldır

            // Diğer açık dropdown'ları kapat
            dropdowns.forEach(otherDropdown => {
                if (otherDropdown !== dropdown && otherDropdown.classList.contains('active')) {
                    otherDropdown.classList.remove('active');
                }
            });
        });
    });
    // Sayfanın herhangi bir yerine tıklandığında dropdown'ları kapat
    window.addEventListener('click', function (event) {
        if (!event.target.matches('.dropbtn') && !event.target.closest('.dropdown-content')) {
            dropdowns.forEach(dropdown => {
                dropdown.classList.remove('active');
            });
        }
    });

    function updateSummary() {
        let secure = document.querySelector('.dropdown.active-dropdown').querySelector('.dropbtn').textContent.trim()

        document.getElementById('summarySecureDisplay').textContent = secure
    }



    // Yardımcı fonksiyon: birden fazla form grubunun görünürlüğünü ayarla
    function displayGroup(groups, display = 'block') {
        groups.forEach(group => {
            if (group) group.style.display = display;
        });
    }

    // HTML elemanlarını seç
    const nonSecureDropdown = document.querySelector('.dropdown:first-of-type .dropdown-content');
    const transactionTypeInput = document.getElementById('transactionType');

    const currencyGroup = document.getElementById('currency')?.closest('.form-group');
    const installmentGroup = document.getElementById('installment')?.closest('.form-group');
    const amountGroup = document.getElementById('amount')?.closest('.form-group');
    const transactionIdGroup = document.getElementById('transactionId')?.closest('.form-group');
    const binGroup = document.getElementById('bin')?.closest('.form-group');
    const lastFourGroup = document.getElementById('lastFourDigits')?.closest('.form-group');
    const tcknVknGroup = document.getElementById('tcknVkn')?.closest('.form-group');
    const transactionType = document.getElementById('transactionType')?.closest('.form-group');

    const secureDropdown = document.querySelector('.dropdown:nth-of-type(2) .dropdown-content');
    const storeTypeInput = document.getElementById('storeType');
    const storeTypeGroup = document.getElementById('storeType')?.closest('.form-group');
    const secureTransactionTypeGroup = document.getElementById('secureTransactionType')?.closest('.form-group');

    if (nonSecureDropdown) {
        const nonSecureLinks = nonSecureDropdown.querySelectorAll('a');

        nonSecureLinks.forEach(link => {
            link.addEventListener('click', event => {
                event.preventDefault();
                updateSummary();

                const typeName = link.textContent.trim();
                const typeUpper = typeName.toUpperCase();
                transactionTypeInput.value = typeUpper;

                // Tüm ekstra alanları gizle
                displayGroup([currencyGroup, installmentGroup, amountGroup, transactionIdGroup], 'none');
                displayGroup([binGroup, lastFourGroup, tcknVknGroup], 'none');
                displayGroup([storeTypeGroup, secureTransactionTypeGroup], 'none');

                switch (typeName) {
                    case 'Ön Provizyon Kapama':
                        displayGroup([transactionIdGroup, amountGroup, transactionType]);
                        break;
                    case 'Moto Insurance':
                        displayGroup([currencyGroup, installmentGroup, amountGroup, transactionType]);
                        displayGroup([binGroup, lastFourGroup, tcknVknGroup]);
                        break;
                    case 'İptal':
                        displayGroup([transactionIdGroup, transactionType]);
                        break;
                    case 'İade':
                        displayGroup([transactionIdGroup, amountGroup, transactionType]);
                        break;
                    case 'Ön Provizyon':
                    case 'Satış':
                    case 'Mail Order':
                        displayGroup([currencyGroup, installmentGroup, amountGroup, transactionType]);
                        break;
                    default:
                        console.warn('Bilinmeyen işlem tipi:', typeName);
                }
            });
        });
    }

    if (secureDropdown) {
        const secureLinks = secureDropdown.querySelectorAll('a');
        secureLinks.forEach(link => {
            link.addEventListener('click', event => {
                event.preventDefault();
                const selectedType = link.textContent.trim().toUpperCase();
                storeTypeInput.value = selectedType;

                displayGroup([currencyGroup, installmentGroup, amountGroup, transactionIdGroup, transactionType], 'none');
                displayGroup([binGroup, lastFourGroup, tcknVknGroup], 'none');

                displayGroup([secureTransactionTypeGroup, storeTypeGroup]);
                displayGroup([currencyGroup, installmentGroup, amountGroup]);

                updateSummary();
            });
        });
    }
});

document.getElementById("amount").addEventListener("input", function () {
    this.value = this.value.replace(/[^0-9,]/g, ''); // Sadece rakam ve virgül
    const parts = this.value.split(',');
    if (parts.length > 2) {
        this.value = parts[0] + ',' + parts[1]; // Fazla virgülü temizle
    }
});

document.addEventListener('DOMContentLoaded', function () {
    // ... (Mevcut kodlarınız buraya kadar devam ediyor) ...

    // Üye İşyeri Numarası input alanını seç
    const merchantNumberInput = document.getElementById('merchantNumber');

    // Sadece Üye İşyeri Numarası için anında sayısal giriş zorlaması
    if (merchantNumberInput) {
        merchantNumberInput.addEventListener('input', function (event) {
            let value = this.value;

            // Sadece rakamları tut (rakam olmayanları kaldır)
            value = value.replace(/[^0-9]/g, '');

            // 15 haneyi geçiyorsa kes (maksimum uzunluk kontrolü)
            if (value.length > 15) {
                value = value.substring(0, 15);
            }

            // Input değerini güncellenmiş haliyle geri yaz
            this.value = value;
        });
    } else {
        console.error("merchantNumber ID'sine sahip HTML elemanı bulunamadı. Lütfen ID'yi kontrol edin.");
    }

    // ... (Geri kalan mevcut JavaScript kodlarınız burada devam ediyor,
    // örneğin NonSecure menüsü için olan kısım ve Sipariş Oluştur butonu için olan kısım) ...

});

document.querySelectorAll('.dropdown-content a').forEach(link => {
    link.addEventListener('click', function (e) {
        // Tüm dropdown'lardan active sınıfını kaldır
        document.querySelectorAll('.dropdown').forEach(drop => {
            drop.classList.remove('active-dropdown');
        });

        // Tıklanan linkin en yakın .dropdown parent'ına active sınıfı ekle
        const dropdown = this.closest('.dropdown');
        if (dropdown) {
            dropdown.classList.add('active-dropdown');
        }

        // İstersen burada seçilen dropdown'ı JS ile kullanabilirsin
        console.log("Seçilen dropdown:", dropdown.querySelector('.dropbtn').textContent.trim());
    });
});

document.addEventListener('DOMContentLoaded', function () {
    // Input alanlarına referanslar
    const merchantNumberInput = document.getElementById('merchantNumber');
    const amountInput = document.getElementById('amount');
    const currencySelect = document.getElementById('currency');

    // Özet görüntüleme elemanlarına referanslar (ID kullanarak)
    const summaryMerchantNumber = document.getElementById('summaryMerchantNumberDisplay');
    const summaryAmount = document.getElementById('summaryAmountDisplay');
    const summaryCurrency = document.getElementById('summaryCurrencyDisplay');
    const summarySecure = document.getElementById('summarySecureDisplay');

    // Özeti güncelleyen fonksiyon
    function updateSummary() {
        summaryMerchantNumber.textContent = merchantNumberInput.value;

        let value = amountInput.value.replace(/[^0-9,]/g, '');
        if (value.trim() === '') value = '0,00';

        updateAmountSummary(value);

        summaryCurrency.textContent = currencySelect.value;

        let secure = document.querySelector('.dropdown.active-dropdown').querySelector('.dropbtn').textContent.trim();
        summarySecure.textContent = secure;
    }

    // Input alanlarındaki değişiklikleri dinle
    merchantNumberInput.addEventListener('input', updateSummary);
    amountInput.addEventListener('input', updateSummary);
    currencySelect.addEventListener('change', updateSummary);

    // Sayfa yüklendiğinde özeti ilk kez güncelle
    updateSummary();
});

window.addEventListener("load", function () {
    // Menüleri seç
    const nonSecureMenu = document.querySelectorAll('.dropdown')[0]; // NonSecure menü
    const secureMenu = document.querySelectorAll('.dropdown')[1];    // Secure menü

    // Önce her iki menüden de active-dropdown sınıfını kaldır
    nonSecureMenu.classList.remove('active-dropdown');
    secureMenu.classList.remove('active-dropdown');

    // localStorage durumuna göre sınıf ekle
    if (localStorage.secureOrderFormData !== undefined && localStorage.secureOrderFormData !== null) {
        secureMenu.classList.add('active-dropdown');
    } else {
        nonSecureMenu.classList.add('active-dropdown');
    }

    let secure = document.querySelector('.dropdown.active-dropdown').querySelector('.dropbtn').textContent.trim()

    localStorage.clear();

    // Input alanlarına referanslar
    const merchantNumberInput = document.getElementById('merchantNumber');
    const amountInput = document.getElementById('amount');
    const currencySelect = document.getElementById('currency');

    // Özet görüntüleme elemanlarına referanslar (ID kullanarak)
    const summaryMerchantNumber = document.getElementById('summaryMerchantNumberDisplay');
    const summaryAmount = document.getElementById('summaryAmountDisplay');
    const summaryCurrency = document.getElementById('summaryCurrencyDisplay');
    const summarySecure = document.getElementById('summarySecureDisplay');

    summaryMerchantNumber.textContent = merchantNumberInput.value;
    // Tutar için virgülü noktaya çevirip formatlama

    const rawValue = amountInput.value.replace(',', '.');
    const parsedValue = parseFloat(rawValue);

    if (!isNaN(parsedValue)) {
        summaryAmount.textContent = parsedValue.toFixed(2);
    } else {
        summaryAmount.textContent = "0,00"; // veya bir uyarı mesajı gösterilebilir
    }

    // Para birimini direkt olarak dropdown'dan gelen değerle güncelle
    summaryCurrency.textContent = currencySelect.value;

    summarySecure.textContent = secure
});


const amountInput = document.getElementById('amount');
const summaryAmount = document.getElementById('summaryAmountDisplay');

// Kullanıcı inputtan çıkınca otomatik virgül ve özet düzeltme
amountInput.addEventListener('blur', function () {
    let value = this.value.replace(/[^0-9,]/g, '');

    // Boşsa 0,00 yap
    if (value.trim() === '') {
        value = '0,00';
    }
    else if (!value.includes(',')) {
        value += ',00';
    } else {
        const parts = value.split(',');
        if (parts.length === 1 || parts[1] === '') {
            value = parts[0] + ',00';
        } else if (parts[1].length === 1) {
            value = parts[0] + ',' + parts[1] + '0';
        } else if (parts[1].length > 2) {
            value = parts[0] + ',' + parts[1].substring(0, 2);
        }
    }

    this.value = value;
    updateAmountSummary(value);
});

// Özet güncelleme fonksiyonu
function updateAmountSummary(inputValue) {
    if (!inputValue || inputValue.trim() === '') {
        summaryAmount.textContent = '0,00';
        return;
    }

    const parts = inputValue.split(',');
    if (parts.length === 1 || parts[1] === '00') {
        summaryAmount.textContent = parts[0] + ',00';
    } else {
        summaryAmount.textContent = parts[0] + ',' + parts[1];
    }
}


document.addEventListener("DOMContentLoaded", function () {
    const merchantNumberInput = document.getElementById("merchantNumber");
    const shopCodeInput = document.getElementById("shopCode");
    const userIdInput = document.getElementById("userId");
    const passwordInput = document.getElementById("password");
    const amountInput = document.getElementById("amount");
    const currencySelect = document.getElementById("currency");

    // Özet alanları
    const summaryMerchantNumberDisplay = document.getElementById("summaryMerchantNumberDisplay");
    const summaryAmountDisplay = document.getElementById("summaryAmountDisplay");
    const summaryCurrencyDisplay = document.getElementById("summaryCurrencyDisplay");
    const summarySecureDisplay = document.getElementById("summarySecureDisplay");

    // Menü seçimleri
    const menuLinks = document.querySelectorAll(".dropdown-content a");

    menuLinks.forEach(link => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const selectedText = this.textContent.trim().toUpperCase();

            if (selectedText === "3D" || selectedText === "3D PAY" || selectedText === "3D PAY HOSTING") {
                handleSecureSelection(selectedText);
                summarySecureDisplay.textContent = `Secure (${selectedText})`;
            } else {
                handleNonSecureSelection();
                summarySecureDisplay.textContent = "NonSecure";
            }

            updateSummary();
        });
    });

    function handleNonSecureSelection() {
        merchantNumberInput.value = "003401000000777";
        shopCodeInput.value = "shopCode005";
        userIdInput.value = "003401000000777_ApiUser";
        passwordInput.value = "1234";
        amountInput.value = "100,00";
    }

    function handleSecureSelection(type) {
        let config = {
            "3D": {
                merchantNumber: "003401000000778",
                shopCode: "shopCode006",
                userId: "003401000000778_ApiUser",
            },
            "3D PAY": {
                merchantNumber: "003401000000779",
                shopCode: "shopCode007",
                userId: "003401000000779_ApiUser",
            },
            "3D PAY HOSTING": {
                merchantNumber: "003401000000780",
                shopCode: "shopCode008",
                userId: "003401000000780_ApiUser",
            }
        };

        const selected = config[type];
        if (selected) {
            merchantNumberInput.value = selected.merchantNumber;
            shopCodeInput.value = selected.shopCode;
            userIdInput.value = selected.userId;
            passwordInput.value = "1234";
            amountInput.value = "100,00";
        }
    }

    function updateSummary() {
        summaryMerchantNumberDisplay.textContent = merchantNumberInput.value;
        summaryAmountDisplay.textContent = amountInput.value;
        summaryCurrencyDisplay.textContent = currencySelect.value;
    }

    // Tutar veya para birimi manuel değişirse özet de güncellensin
    amountInput.addEventListener("input", updateSummary);
    currencySelect.addEventListener("change", updateSummary);
});
