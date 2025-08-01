

window.addEventListener('DOMContentLoaded', () => {
    // localStorage'dan verileri al
    const orderFormDataRaw = localStorage.getItem('orderFormData');
    const paymentResultRaw = localStorage.getItem('paymentResult');
    const paymentRequestRaw = localStorage.getItem('secureRequest');
    const secureOrderFormDataRaw = localStorage.getItem('secureOrderFormData');

    function getQueryParam(param) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }

    const jsonDataStr = getQueryParam('data');

    if (orderFormDataRaw && paymentResultRaw) {
        const orderFormData = JSON.parse(orderFormDataRaw);
        const paymentResult = JSON.parse(paymentResultRaw);
        const paymentRequest = JSON.parse(paymentRequestRaw);

        // orderFormData verilerini yerleştir
        document.getElementById('summaryMerchantNumberDisplay').textContent = orderFormData.merchantNumber;
        document.getElementById('summaryTransactionTypeDisplay').textContent = orderFormData.transactionType;
        document.getElementById('summaryMerchantShopCodeDisplay').textContent = orderFormData.shopCode;
        document.getElementById('summaryAmountDisplay').textContent = orderFormData.amount;
        document.getElementById('summaryCurrencyDisplay').textContent = orderFormData.currency;

        if (paymentRequest) {
            document.getElementById('summaryMerchantNumberDisplay').textContent = paymentRequest.merchantNumber;
            document.getElementById('summaryAmountDisplay').textContent = paymentRequest.transactionAmount;
            document.getElementById('summaryCurrencyDisplay').textContent = paymentRequest.currencyCode;

            let transactionLabel = "";

            if (paymentRequest.transactionType === "SALEPOS") {
                transactionLabel = "Satış";
            } else if (paymentRequest.transactionType === "PREAUTH") {
                transactionLabel = "Ön Provizyon";
            } else {
                transactionLabel = "Bilinmeyen İşlem";
            }

            document.getElementById('summaryTransactionTypeDisplay').textContent = transactionLabel;
        }

        if (paymentResult) {
            // paymentResult verilerini yerleştir
            document.getElementById('summaryResponseCodeDisplay').textContent = paymentResult.responseCode;
            document.getElementById('summaryResponseReasonCodeDisplay').textContent = paymentResult.responseReasonCode;
            document.getElementById('summaryResponseMessageDisplay').textContent = paymentResult.responseMessage;
            document.getElementById('summaryOrderIdDisplay').textContent = paymentResult.orderId;
            document.getElementById('summaryAuthorizationNumberDisplay').textContent = paymentResult.authorizationNumber;
            document.getElementById('summaryRRNDisplay').textContent = paymentResult.rrn;
            document.getElementById('summaryStanDisplay').textContent = paymentResult.stan;
            document.getElementById('summaryTransactionIdDisplay').textContent = paymentResult.transactionId;
            document.getElementById('summaryTransactionDateDisplay').textContent = new Date(paymentResult.transactionDate).toLocaleString('tr-TR');
            //document.getElementById('summaryIsErrorDisplay').textContent = paymentResult.isError ? 'Evet' : 'Hayır';
            //document.getElementById('summarySuccessDisplay').textContent = paymentResult.success ? 'Evet' : 'Hayır';

            const resultStatus = document.getElementById('resultStatus');
            if (paymentResult.success) {
                resultStatus.textContent = 'Onaylandı';
                resultStatus.classList.add('success');
            } else {
                resultStatus.textContent = 'Onaylanmadı';
                resultStatus.classList.add('fail');
            }
        }
        else {
            // summary alanlarını gizle
            document.getElementById('summaryContainer').style.display = 'none';
        }

        const secureRequestData = localStorage.getItem('secureRequest');

        const secureRequest = JSON.parse(secureRequestData);

        function setText(id, text) {
            const el = document.getElementById(id);
            if (el) el.textContent = text ?? "";
        }

        if (secureRequest) {
            setText("av", secureRequest.av);
            setText("eci", secureRequest.eci);
            setText("threeDModel", secureRequest.threeDModel);


            //setText("TransactionId", secureRequest.transactionId);
            //setText("OrderId", secureRequest.orderId);
            setText("summaryMerchantShopCodeDisplay", secureRequest.shopCode);
        } else {
            document.getElementById('summaryContainer').style.display = 'none';
        }

    }
    else if (jsonDataStr) {
        const data = JSON.parse(jsonDataStr);
        const secureOrderFormData = JSON.parse(secureOrderFormDataRaw);

        // paymentResult verilerini yerleştir
        document.getElementById('summaryResponseCodeDisplay').textContent = data.Result.Code;
        document.getElementById('summaryResponseReasonCodeDisplay').textContent = data.Result.ReasonCode;
        document.getElementById('summaryResponseMessageDisplay').textContent = data.Result.Message;
        document.getElementById('summaryOrderIdDisplay').textContent = data.TransactionResponse.OrderId;
        document.getElementById('summaryAuthorizationNumberDisplay').textContent = data.AuthenticationResult.AuthorizationNumber;
        document.getElementById('summaryRRNDisplay').textContent = data.AuthenticationResult.RRN;
        document.getElementById('summaryStanDisplay').textContent = data.AuthenticationResult.Stan;
        document.getElementById('summaryTransactionIdDisplay').textContent = data.TransactionResponse.TransactionId;
        document.getElementById('summaryTransactionDateDisplay').textContent = new Date(data.TransactionResponse.TransactionDate).toLocaleString('tr-TR');
        //document.getElementById('summaryIsErrorDisplay').textContent = data.isError ? 'Evet' : 'Hayır';
        //document.getElementById('summarySuccessDisplay').textContent = data.success ? 'Evet' : 'Hayır';

        const resultStatus = document.getElementById('resultStatus');
        if (data.Result.Code == "000000") {
            resultStatus.textContent = 'Onaylandı';
            resultStatus.classList.add('success');
        } else {
            resultStatus.textContent = 'Onaylanmadı';
            resultStatus.classList.add('fail');
        }

        if (secureOrderFormData) {
            document.getElementById('summaryMerchantNumberDisplay').textContent = secureOrderFormData.merchantNumber;
            document.getElementById('summaryAmountDisplay').textContent = secureOrderFormData.amount;
            document.getElementById('summaryCurrencyDisplay').textContent = secureOrderFormData.currency;
            document.getElementById('summaryMerchantShopCodeDisplay').textContent = secureOrderFormData.shopCode;
            document.getElementById('summaryTransactionTypeDisplay').textContent = secureOrderFormData.transactionType;
            document.getElementById('threeDModel').textContent = secureOrderFormData.storeType;
        } else {

            document.getElementById('summaryContainer').style.display = 'none';
        }
    }
    else {
        console.warn('localStorage verileri bulunamadı.');
    }
});
