/**
 * 
 * @param {string} formElementId 
 * @param {string} orderDirectionButtonId 
 * @param {string} orderDirectionInputId 
 */
function useUserListController(
        formElementId,
        orderDirectionButtonId,
        orderDirectionInputId) {
    /**
     * @type {HTMLFormElement}
     */
    const formElement = document.getElementById(formElementId);
    
    /**
     * @type {HTMLButtonElement}
     */
    const orderDirectionButtonElement = document.getElementById(orderDirectionButtonId);
    
    /**
     * @type {HTMLInputElement}
     */
    const orderDirectionInputElement = document.getElementById(orderDirectionInputId)
    orderDirectionButtonElement.addEventListener("click", () => {
        if (orderDirectionInputElement.value === "True") {
            orderDirectionButtonElement.value = "False";
        } else {
            orderDirectionButtonElement.value = "True";
        }
        formElement.submit();
    })
}