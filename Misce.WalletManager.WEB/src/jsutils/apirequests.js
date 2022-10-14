const commonPostHeader = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
};

export function getApiBaseUrl() {
    return 'https://localhost:7264/api/'
};

export function getLoginPostSettings(loginForm) {
    return {
        method: 'POST',
        headers: commonPostHeader,
        body: JSON.stringify({
            username: loginForm.username,
            password: loginForm.password
        })
    }
}