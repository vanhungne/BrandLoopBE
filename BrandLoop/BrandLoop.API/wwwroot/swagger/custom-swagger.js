window.onload = function () {
    const ui = SwaggerUIBundle({
        url: "/swagger/v1/swagger.json",
        dom_id: '#swagger-ui',
        presets: [
            SwaggerUIBundle.presets.apis,
            SwaggerUIBundle.plugins.DownloadUrl
        ],
        onComplete: function () {
            const authorizeButton = document.querySelector('.authorize');
            authorizeButton.addEventListener('click', function () {
                const cookies = document.cookie.split('; ');
                let cookieValue = '';
                cookies.forEach(function (cookie) {
                    if (cookie.startsWith('accessToken=')) {
                        cookieValue = cookie.split('=')[1];
                    }
                });
                console.log("Token:", cookieValue); // Debug token
                document.querySelectorAll('input[name="Authorization"]').forEach(function (input) {
                    input.value = "Bearer " + cookieValue;
                });
            });

                });
            });
        }
    });
};
