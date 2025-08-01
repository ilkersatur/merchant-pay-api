document.addEventListener("DOMContentLoaded", function () {
    const urlParams = new URLSearchParams(window.location.search);
    const responseStr = urlParams.get("data");

    if (!responseStr) {
        alert("Herhangi bir sonuç verisi bulunamadı.");
        return;
    }

    let responseJson;
    try {
        responseJson = JSON.parse(responseStr);
    } catch (e) {
        alert("JSON verisi çözümlenemedi.");
        return;
    }

    const authResult = responseJson.ThreeDSAuthenticationResult || {};
    const transResponse = responseJson.TransactionResponse || {};
    const result = responseJson.Result || {};

    document.getElementById("av").innerText = authResult.Av || "-";
    document.getElementById("eci").innerText = authResult.Eci || "-";

    document.getElementById("transactionId").innerText = transResponse.TransactionId || "-";
    document.getElementById("orderId").innerText = transResponse.OrderId || "-";
    document.getElementById("transactionDate").innerText = transResponse.TransactionDate
        ? new Date(transResponse.TransactionDate).toLocaleString("tr-TR")
        : "-";

    document.getElementById("code").innerText = result.Code || "-";
    document.getElementById("reasonCode").innerText = result.ReasonCode || "-";
    document.getElementById("message").innerText = result.Message || "-";
});


document.addEventListener("DOMContentLoaded", () => {
    const approveButton = document.getElementById("Approve");

    approveButton.addEventListener("click", async () => {

        document.getElementById("loading").style.display = "block";

        const paymentResult = localStorage.getItem('paymentResult');
        const secureOrderFormData = localStorage.getItem('secureOrderFormData');

        const paymentData = JSON.parse(paymentResult);
        const secureOrder = JSON.parse(secureOrderFormData);

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

        // Değerleri HTML'den al
        const requestData = {
            //ThreeDsDirectoryServerTransactionId: document.getElementById("threeDSServerTransID")?.textContent || "",
            //ThreeDsProgramProtocol: document.getElementById("authenticationProtocol")?.textContent || "",
            //dsTransID: document.getElementById("dsTransID")?.textContent || "",
            //acsTransID: document.getElementById("acsTransID")?.textContent || "",
            //threeDSTransStatus: document.getElementById("threeDSTransStatus")?.textContent || "",
            //threeDSTransStatusReason: document.getElementById("threeDSTransStatusReason")?.textContent || "",
            av: document.getElementById("av")?.textContent || "",
            eci: document.getElementById("eci")?.textContent || "",
            transactionId: document.getElementById("transactionId")?.textContent || "",
            orderId: document.getElementById("orderId")?.textContent || "",
            userId: secureOrder.userId,
            password: secureOrder.password,
            merchantNumber: secureOrder.merchantNumber,
            shopCode: secureOrder.shopCode,
            transactionType: mapTransactionType(secureOrder.transactionType),
            transactionAmount: String(parseInt(secureOrder.amount) * 100),
            currencyCode: secureOrder.currency == "TRY" ? "949" : secureOrder.currency,
            installmentCount: secureOrder.installment,
            threeDModel: secureOrder.storeType,
        };

        // localStorage'dan form verisi çekiyorsan örnek:
        const orderData = JSON.parse(localStorage.getItem("orderFormData") || "{}");

        try {
            const response = await fetch("https://localhost:7186/api/Secure/Auth3DS", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(requestData)
            });

            const result = await response.json();
            console.log("Yanıt:", result);

            localStorage.clear();
            localStorage.setItem("orderFormData", JSON.stringify(orderData));
            localStorage.setItem("paymentResult", JSON.stringify(result));
            localStorage.setItem("secureRequest", JSON.stringify(requestData));

            window.location.href = "result.html";
        } catch (error) {
            console.error("İstek hatası:", error);
            alert("İstek gönderilirken hata oluştu.");
        } finally {
            document.getElementById("loading").style.display = "none"; // Gizle
        }
    });
});
