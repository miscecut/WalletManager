const commonPostHeader = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
};

function getPostSettings(bodyData, token = null) {
    let postHeader = commonPostHeader;
    if (token != null)
        postHeader['Authorization'] = 'Bearer ' + token;
    return {
        method: 'POST',
        headers: postHeader,
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

export function getTransactionCategoryCreatePostSettings(transactionCategoryForm, token) {
    return getPostSettings({
        name: transactionCategoryForm.name,
        isExpenseType: transactionCategoryForm.isExpenseType,
    }, token);
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

export function getTransactionsGetQueryParameters(transactionsFilters) {
    let parameters = {
        limit: 1000,
        page: 0
    };
    if (transactionsFilters.transactionType != null && transactionsFilters.transactionType != '')
        parameters.transactionType = transactionsFilters.transactionType;
    if (transactionsFilters.fromAccountId != null && transactionsFilters.fromAccountId != '')
        parameters.accountFromId = transactionsFilters.fromAccountId;
    if (transactionsFilters.toAccountId != null && transactionsFilters.toAccountId != '')
        parameters.accountToId = transactionsFilters.toAccountId;
    if (transactionsFilters.transactionCategoryId != null && transactionsFilters.transactionCategoryId != '')
        parameters.transactionCategoryId = transactionsFilters.transactionCategoryId;
    if (transactionsFilters.transactionSubCategoryId != null && transactionsFilters.transactionSubCategoryId != '')
        parameters.transactionSubCategoryId = transactionsFilters.transactionSubCategoryId;
    if (transactionsFilters.fromDate != null && transactionsFilters.fromDate != '')
        parameters.dateFrom = transactionsFilters.fromDate;
    //return query
    return ('?' + new URLSearchParams(parameters));
}

export function getTransactionSubCategoriesGetQueryParameters(transactionCategoryId) {
    let parameters = {
        limit: 1000,
        page: 0
    };
    //transaction category id
    if (transactionCategoryId != null && transactionCategoryId != '')
        parameters.transactionCategoryId = transactionCategoryId;
    //return query
    return ('?' + new URLSearchParams(parameters));
}