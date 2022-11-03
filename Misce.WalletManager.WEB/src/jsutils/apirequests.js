const commonPostHeader = {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
};

function getPostSettings(bodyData, token = null, httpMethod = 'POST') {
    console.log(bodyData);
    let postHeader = commonPostHeader;
    if (token != null)
        postHeader['Authorization'] = 'Bearer ' + token;
    return {
        method: httpMethod,
        headers: postHeader,
        body: JSON.stringify(bodyData)
    }
}

export function getApiBaseUrl() {
    return 'https://localhost:7264/api/'
};

//PUT requests

export function getTransactionCategoryUpdateSettings(transactionCategoryForm, token) {
    return getPostSettings({
        name: transactionCategoryForm.name
    }, token, 'PUT');
}

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

export function getTransactionCreatePostSettings(transactionForm, token) {
    return getPostSettings({
        title: transactionForm.title,
        transactionSubCategoryId: transactionForm.transactionSubCategoryId === '' ? null : transactionForm.transactionSubCategoryId,
        amount: 3.3,
        fromAccountId: transactionForm.accountFromId === '' ? null : transactionForm.accountFromId,
        toAccountId: transactionForm.accountToId === '' ? null : transactionForm.accountToId,
        description: transactionForm.description,
        dateTime: transactionForm.dateTime
    }, token);
}

export function getTransactionCategoryCreatePostSettings(transactionCategoryForm, token) {
    return getPostSettings({
        name: transactionCategoryForm.name,
        isExpenseType: transactionCategoryForm.isExpenseType,
    }, token);
}

export function getTransactionSubCategoryCreatePostSettings(transactionSubCategoryForm, token) {
    return getPostSettings({
        name: transactionSubCategoryForm.name,
        transactionCategoryId: transactionSubCategoryForm.transactionCategoryId,
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

export function getTransactionSubCategoriesGetQueryParameters(parameters) {
    let transformedParameters = parameters.transactionCategoryId != null && parameters.transactionCategoryId != '' ? {
        transactionCategoryId: parameters.transactionCategoryId
    } : null;
    if (transformedParameters == null)
        return '';
    return ('?' + new URLSearchParams(transformedParameters));
}

export function getTransactionCategoriesGetQueryParameters(transactionType) {
    let isExpenseTypeParameter = transactionType != null && transactionType != '' ? {
        isExpenseType: transactionType === '0'
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