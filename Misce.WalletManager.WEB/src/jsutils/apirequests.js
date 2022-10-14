const commonPostHeader = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
};

function getPostSettings(bodyData) {
    return {
        method: 'POST',
        headers: commonPostHeader,
        body: JSON.stringify(bodyData)
    }
}

export function getApiBaseUrl() {
    return 'https://localhost:7264/api/'
};

export function getLoginPostSettings(loginForm) {
    return getPostSettings({
        username: loginForm.username,
        password: loginForm.password
    });
}

export function getRegisterPostSettings(registerForm) {
    return getPostSettings({
        username: registerForm.username,
        password: registerForm.password,
        confirmPassword: registerForm.confirmPassword
    });
}