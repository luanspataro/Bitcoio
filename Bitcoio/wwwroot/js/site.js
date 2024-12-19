document.getElementById("bitcoinForm").addEventListener("submit", function (event) {
    event.preventDefault();

    var form = event.target;
    var formData = new FormData(form);
    var jsonData = {};

    formData.forEach((value, key) => {
        jsonData[key] = value;
    });

    var submitButton = form.querySelector('button[type="submit"]');
    submitButton.disabled = true;

    fetch("/Bitcoin/GetBitcoinPrice", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(jsonData)
    })
        .then(response => response.json())
        .then(data => {
            var resultDiv = document.getElementById("result");
            if (data.success) {
                var existingIframe = document.querySelector('iframe.animation-iframe');
                if (existingIframe) {
                    existingIframe.remove();
                }

                const element = document.querySelector('.element');
                const animationIframe = document.createElement('iframe');
                animationIframe.src = "https://lottie.host/embed/5f28d8e3-c237-4dad-af92-c774bfdb6130/gwbWvm54N5.json";
                animationIframe.className = 'animation-iframe';
                animationIframe.style.width = "55vw";
                animationIframe.style.height = "35vh";
                animationIframe.style.border = "none";
                animationIframe.style.position = "absolute";
                animationIframe.style.top = "50%";
                animationIframe.style.left = "50%";
                animationIframe.style.transform = "translate(-50%, -50%)";
                animationIframe.style.display = "none";
                document.body.appendChild(animationIframe);

                element.classList.add('shrink');

                animationIframe.style.display = 'block';
                requestAnimationFrame(() => {
                    animationIframe.classList.add('show');
                });

                const resultHTML = `
                <div class="mb-0 result-content">
                    <div class="row justify-content-center">
                        <div class="col-4">
                            <h4 class="mt-3">Quantidade</h4>
                            <p>${data.amount} BTC</p>
                        </div>

                        <div class="col-4">
                            <h4 class="mt-3">Total</h4>
                            <p>${data.total}</p>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div class="col-4">
                            <h4 class="">Valorização</h4>
                            <p class="${data.percentage >= 0 ? 'profit' : 'loss'}">
                                ${data.percentage.toFixed(2)} %
                            </p>
                        </div>

                        <div class="col-4">
                            <h4 class="">Lucro</h4>
                            <p class="${data.percentage >= 0 ? 'profit' : 'loss'}">
                                ${data.profit}
                            </p>
                        </div>
                    </div>
                </div>`;

                const initialText = document.querySelector('.initial-text');
                initialText.classList.add('move-up');


                setTimeout(() => {

                    animationIframe.classList.add('hide');

                    setTimeout(() => {

                        element.classList.remove('shrink');
                        element.classList.add('grow');

                        animationIframe.style.display = 'none';

                        resultDiv.innerHTML = resultHTML;
                        initialText.classList.remove('move-up');
                        initialText.classList.add('move-down');

                        setTimeout(() => {

                            element.classList.remove('grow');
                            submitButton.disabled = false;

                        }, 300);
                    }, 1000);
                }, 2700);
            } else {
                resultDiv.innerHTML = `<p class="text-danger">Erro: ${data.error}</p>`;
                submitButton.disabled = false;
            }
        })
        .catch(error => {
            console.error("Erro:", error);
            submitButton.disabled = false;
        });
});
