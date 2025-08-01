document.addEventListener('DOMContentLoaded', () => {
    // Mevcut buton olayları
    const createOrderBtn = document.getElementById('createOrderBtn');


    if (createOrderBtn) {
        createOrderBtn.addEventListener('click', async () => {

            const requiredFields = [];

            // Ortak alanlar
            const name = document.getElementById("nameInput");
            const card = document.getElementById("cardInput");
            const month = document.getElementById("expiry-month-input");
            const year = document.getElementById("expiry-year-input");
            const cvv = document.getElementById("cvvInput");

            // Temel zorunlu alanlar
            requiredFields.push(name, card, month, year, cvv);

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
                const storedData = localStorage.getItem('orderFormData');
                const secureStoredData = localStorage.getItem('secureOrderFormData');

                const orderData = JSON.parse(storedData);
                const secureOrderData = JSON.parse(secureStoredData);

                const mapTransactionType = (type) => {
                    switch (type) {
                        case "SATIŞ":
                        case "Satış":
                            return "SALEPOS";
                        case "ÖN PROVIZYON":
                        case "Ön Provizyon":
                            return "PREAUTH";
                        case "MAIL ORDER":
                            return "MAILORDER";
                        default:
                            return type;
                    }
                };

                const mapStoreType = (type) => {
                    switch (type) {
                        case "3D":
                            return "3D";
                        case "3D PAY":
                            return "3D_PAY";
                        case "3D PAY HOSTING":
                            return "3D_PAY_HOSTING";
                        default:
                            return type;
                    }
                };

                function getExpireDate() {
                    const yearFull = document.getElementById('expiry-year-input').value; // örn: "2028"
                    const month = document.getElementById('expiry-month-input').value; // örn: "01"

                    if (!yearFull || !month) {
                        return "";
                    }

                    const yearShort = yearFull.slice(-2); // son 2 haneyi al => "28"

                    return yearShort + month; // "2801"
                }

                const requestData = {
                    userId: orderData ? orderData.userId : secureOrderData.userId,
                    password: orderData ? orderData.password : secureOrderData.password,
                    merchantNumber: orderData ? orderData.merchantNumber : secureOrderData.merchantNumber,
                    shopCode: orderData ? orderData.shopCode : secureOrderData.shopCode,
                    transactionType: mapTransactionType(orderData ? orderData.transactionType : secureOrderData.secureTransactionType),
                    cardHolderName: document.getElementById("nameInput").value,
                    transactionAmount: (() => {
                        const rawAmount = (orderData ? orderData.amount : secureOrderData.amount)?.replace(',', '.');
                        const parsedAmount = parseFloat(rawAmount);
                        const amountInCents = Math.round(parsedAmount * 100);
                        return isNaN(amountInCents) ? null : String(amountInCents);
                    })(),
                    // currencyCode: orderData.currency == "TRY" ? "949" : "NaN",
                    currencyCode: "949",
                    pan: document.getElementById("cardInput").value.replace(/\s+/g, ''),
                    cvv2: document.getElementById("cvvInput").value,
                    expireDate: getExpireDate(),
                    installmentCount: orderData ? orderData.installment : secureOrderData.installment,
                    securityType: orderData ? 'NonSecure' : 'Secure',
                    rewardAmount: "0",
                    storeType: secureOrderData ? mapStoreType(secureOrderData?.storeType) : null,
                    AcquirerMerchantId: orderData ? orderData.merchantNumber : secureOrderData.merchantNumber
                };

                document.getElementById("loading").style.display = "block";

                try {
                    if (orderData) {
                        const response = await fetch("https://localhost:7186/api/Payment", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json"
                            },
                            body: JSON.stringify(requestData)
                        });

                        const result = await response.json();
                        console.log("Yanıt:", result);

                        localStorage.setItem("paymentResult", JSON.stringify(result));


                        window.location.href = "result.html";

                        // alert("İşlem sonucu: " + (result.success ? "Başarılı" : "Hata"));
                    } else if (secureOrderData) {
                        const response = await fetch("https://localhost:7186/api/Secure", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json"
                            },
                            body: JSON.stringify(requestData)
                        });

                        const result = await response.json();
                        console.log("Yanıt:", result);

                        localStorage.setItem("paymentResult", JSON.stringify(result));

                        const form = document.getElementById("paymentForm");

                        for (const key in result) {
                            const input = form.querySelector(`input[name="${key}"]`);
                            if (input) {
                                input.value = result[key];
                            }
                        }

                        form.submit();

                    }
                } catch (error) {
                    console.error("İstek hatası:", error);
                    alert("İstek gönderilirken hata oluştu.");
                }
                finally {
                    document.getElementById("loading").style.display = "none"; // Gizle
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

    // Kart Numarası Formatlama İşlevi
    const formattedCardNumberInput = document.getElementById('cardInput');

    if (formattedCardNumberInput) {
        formattedCardNumberInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\s/g, ''); // Mevcut boşlukları kaldır
            let formattedValue = '';

            // Sadece rakamları al
            value = value.replace(/\D/g, '');

            // Her 4 karakterden sonra boşluk ekle
            for (let i = 0; i < value.length; i++) {
                if (i > 0 && i % 4 === 0) {
                    formattedValue += ' ';
                }
                formattedValue += value[i];
            }

            e.target.value = formattedValue;
        });

        // Maksimum karakter sayısı (16 rakam + 3 boşluk = 19)
        formattedCardNumberInput.setAttribute('maxlength', '19');
    }
});


function generateCard() {
    const prefixes = ["52", "34", "4", "9792"];

    // Rastgele bir prefix seç
    const prefix = prefixes[Math.floor(Math.random() * prefixes.length)];

    // Geri kalan basamak sayısı
    const totalDigits = 16;
    const remainingLength = totalDigits - prefix.length;

    // Geri kalan rakamları oluştur
    let number = prefix;
    for (let i = 0; i < remainingLength; i++) {
        number += Math.floor(Math.random() * 10);
    }

    // 4’lü gruplara böl (4000 1234 5678 9012 gibi)
    const formattedNumber = number.match(/.{1,4}/g).join(" ");

    // Input ve kart yüzüne yaz
    document.getElementById("cardInput").value = formattedNumber;
    document.getElementById("card-number").textContent = formattedNumber;

    // Kart tipi ve renk güncelle
    updateCardTypeDisplay(formattedNumber);
}



// Form alanlarını dinleyerek kart görselini güncelle

document.getElementById("nameInput").addEventListener("input", function () {
    const upperName = this.value.toUpperCase();
    document.querySelector(".card-name").textContent = upperName || "ILKER SATUR";
});


document.getElementById("cardInput").addEventListener("input", function () {
    document.getElementById("card-number").textContent = this.value;
});

//   document.getElementById("expiry-input").addEventListener("input", function () {
//     document.querySelector(".card-expiry").textContent = "VALID THRU ► " + (this.value || "01/23");
//   });


function detectCardType(number) {
    const cleaned = number.replace(/\s+/g, '');
    if (/^4/.test(cleaned)) return "VISA";
    if (/^5[1-5]/.test(cleaned) || /^2(2[2-9]|[3-6][0-9]|7[01])/.test(cleaned)) return "MASTERCARD";
    if (/^3[47]/.test(cleaned)) return "AMEX";
    if (/^9792/.test(cleaned)) return "TROY";
    return "";
}

function updateCardTypeDisplay(number) {
    const type = detectCardType(number);
    const logoImg = document.getElementById("card-logo-img");
    const card = document.getElementById("card");

    // Kart tipi sınıflarını temizle
    card.classList.remove("visa", "mastercard", "troy", "amex");

    // Varsayılan logo görünmesin
    logoImg.style.display = "inline";

    switch (type) {
        case "VISA":
            card.classList.add("visa");
            logoImg.src = "assets/images/visa.png";
            logoImg.alt = "Visa";
            break;
        case "MASTERCARD":
            card.classList.add("mastercard");
            logoImg.src = "assets/images/mastercard.png";
            logoImg.alt = "MasterCard";
            break;
        case "TROY":
            card.classList.add("troy");
            logoImg.src = "assets/images/troy.png";
            logoImg.alt = "Troy";
            break;
        case "AMEX":
            card.classList.add("amex");
            logoImg.src = "assets/images/amex.png";
            logoImg.alt = "Amex";
            break;
        default:
            logoImg.style.display = "none"; // bilinmeyense gizle
            break;
    }
}


document.getElementById("cardInput").addEventListener("input", function () {
    const value = this.value;
    document.getElementById("card-number").textContent = value;
    updateCardTypeDisplay(value);
});

const cvvInput = document.querySelector('input[placeholder="000"]');
const cardElement = document.getElementById("card");
const backName = document.getElementById("back-name");
const cvvDisplay = document.getElementById("cvv-display");
const nameInput = document.getElementById("nameInput");

// CVV'ye odaklanıldığında kartı çevir
cvvInput.addEventListener("focus", function () {
    cardElement.classList.add("flipped");
});

// CVV'den çıkıldığında kartı geri çevir
cvvInput.addEventListener("blur", function () {
    cardElement.classList.remove("flipped");
});

// CVV değeri yazıldıkça arka yüzde göster
cvvInput.addEventListener("input", function () {
    cvvDisplay.textContent = this.value || "000";
});

// Ad soyad değiştikçe arka yüzdeki imza alanına da yaz
nameInput.addEventListener("input", function () {
    backName.textContent = this.value || "ILKER SATUR";
});

document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('cardInput');

    input.addEventListener('input', (e) => {
        let value = e.target.value;

        // Sayı olmayanları kaldır
        value = value.replace(/\D/g, '');

        // Maksimum 16 rakam al
        if (value.length > 16) {
            value = value.slice(0, 16);
        }

        // 4'erli bloklara ayır (boşluk ile)
        let formattedValue = '';
        for (let i = 0; i < value.length; i++) {
            if (i > 0 && i % 4 === 0) {
                formattedValue += ' ';
            }
            formattedValue += value[i];
        }

        e.target.value = formattedValue;
    });
});



const monthSelect = document.getElementById('expiry-month-input');
const yearSelect = document.getElementById('expiry-year-input');
const expiryDisplay = document.getElementById('card-expiry');

function updateExpiry() {
    const month = monthSelect.value;
    const year = yearSelect.value;
    if (month && year) {
        expiryDisplay.textContent = `VALID THRU ► ${month}/${year}`;
    }
}

monthSelect.addEventListener('change', updateExpiry);
yearSelect.addEventListener('change', updateExpiry);



document.addEventListener('DOMContentLoaded', () => {
    // Get the stored data from Local Storage
    const storedData = localStorage.getItem('orderFormData');
    const secureStoredData = localStorage.getItem('secureOrderFormData');

    // Parse the JSON string back into a JavaScript object
    const orderData = JSON.parse(storedData);
    const secureOrderData = JSON.parse(secureStoredData);

    // Get the span elements where you want to display the data
    const merchantNumberDisplay = document.getElementById('summaryMerchantNumberDisplay');
    const transactionTypeDisplay = document.getElementById('summaryTransactionTypeDisplay');
    const amountDisplay = document.getElementById('summaryAmountDisplay');
    const currencyDisplay = document.getElementById('summaryCurrencyDisplay');
    const secureDisplay = document.getElementById('summarySecureDisplay');

    // Populate the span elements with the retrieved data
    if (merchantNumberDisplay) {
        merchantNumberDisplay.textContent = orderData ? orderData.merchantNumber : secureOrderData.merchantNumber;
    }
    if (transactionTypeDisplay) {
        transactionTypeDisplay.textContent = orderData ? orderData.transactionType : secureOrderData.transactionType;
    }
    if (amountDisplay) {
        amountDisplay.textContent = orderData ? orderData.amount : secureOrderData.amount;
    }
    if (currencyDisplay) {
        currencyDisplay.textContent = orderData ? orderData.currency : secureOrderData.currency;
    }
    if (secureDisplay) {
        secureDisplay.textContent = orderData ? 'NonSecure' : 'Secure';
    }


    const header = document.querySelector('header'); // Header elementini seçin
    if (secureOrderData && secureOrderData.storeType === "3D PAY HOSTING") {
        const bankLogo = document.createElement('img');
        bankLogo.src = 'assets/images/bank-logo.png'; // Logoun yolu
        bankLogo.alt = 'Banka Logosu';
        bankLogo.style.height = '30px'; // Logoun yüksekliği
        bankLogo.style.marginLeft = 'auto'; // Flexbox ile en sağa hizalama
        bankLogo.style.alignSelf = 'center'; // Dikey hizalama için
        if (header) { // Header'ın var olduğundan emin olun
            header.appendChild(bankLogo);
        }
    }

});