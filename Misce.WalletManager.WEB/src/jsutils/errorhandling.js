//this function accepts an array of object of the type: { field: "...", message: "..." } and returns a map field => message
export function getErrorMap(errors) {
    let errorMap = {}
    errors.forEach(error => errorMap[error.field.toLowerCase()] = error.message);
    return errorMap;
}