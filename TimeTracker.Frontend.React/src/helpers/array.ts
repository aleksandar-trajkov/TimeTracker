/**
 * Helper functions for array operations
 */

/**
 * Groups an array of objects by a specified key or key function
 * @param array - The array to group
 * @param keyOrFunction - Either a key name or a function that returns the grouping key
 * @returns An object with keys as group identifiers and values as arrays of grouped items
 */
export function groupBy<T, K extends string | number | symbol>(
    array: T[],
    keyOrFunction: keyof T | ((item: T) => K)
): Record<K, T[]> {
    return array.reduce((acc, item) => {
        const key = typeof keyOrFunction === 'function' 
            ? keyOrFunction(item)
            : (item[keyOrFunction] as unknown as K);
        
        if (!acc[key]) {
            acc[key] = [];
        }
        acc[key].push(item);
        return acc;
    }, {} as Record<K, T[]>);
}