

window.addEventListener('DOMContentLoaded', () => {

    function getQueryParam(param) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }

    const jsonDataStr = getQueryParam('data');

    if (jsonDataStr) {
        try {
            const data = JSON.parse(jsonDataStr);

            function setText(id, text) {
                const el = document.getElementById(id);
                if (el) el.textContent = text ?? "";
            }

            setText("threeDSServerTransID", data.threeDSServerTransID);
            setText("dsTransID", data.dsTransID);
            setText("acsTransID", data.acsTransID);
            setText("threeDSTransStatus", data.threeDSTransStatus === "Y" ? "Y-Başarılı" : data.threeDSTransStatus);
            setText("threeDSTransStatusReason", data.threeDSTransStatusReason);
            setText("authenticationProtocol", data.authenticationProtocol);

            if (data.authenticationResult) {
                setText("av", data.authenticationResult.av);
                setText("eci", data.authenticationResult.eci);
            }

            setText("TransactionId", data.TransactionId);
            setText("OrderId", data.OrderId);

        } catch (e) {
            console.error("JSON parse hatası:", e);
        }
    } else {
        console.warn("URL query string içinde 'data' parametresi yok.");
    }
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
            ThreeDsDirectoryServerTransactionId: document.getElementById("threeDSServerTransID")?.textContent || "",
            ThreeDsProgramProtocol: document.getElementById("authenticationProtocol")?.textContent || "",
            dsTransID: document.getElementById("dsTransID")?.textContent || "",
            acsTransID: document.getElementById("acsTransID")?.textContent || "",
            threeDSTransStatus: document.getElementById("threeDSTransStatus")?.textContent || "",
            threeDSTransStatusReason: document.getElementById("threeDSTransStatusReason")?.textContent || "",
            av: document.getElementById("av")?.textContent || "",
            eci: document.getElementById("eci")?.textContent || "",
            transactionId: document.getElementById("TransactionId")?.textContent || "",
            orderId: document.getElementById("OrderId")?.textContent || "",
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
