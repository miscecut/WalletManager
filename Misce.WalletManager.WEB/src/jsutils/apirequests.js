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

//POST requests

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

//GET requests

export function getGetCommonSettings(token) {
    return {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token
        }
    }
}

export function getTransactionCategoriesGetQueryParameters(transactionType) {
    let isExpenseTypeParameter = transactionType != null && transactionType != '' ? {
        isExpenseType: transactionType === 'EXPENSE'
    } : null;
    if (isExpenseTypeParameter == null)
        return '';
    return ('?' + new URLSearchParams(isExpenseTypeParameter));
}