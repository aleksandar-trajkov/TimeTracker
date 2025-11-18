import { useQuery } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executeGet } from '../../helpers/fetch';
import type { Category } from '../../types/Category';

const useCategoriesQuery = (organizationId: string) => {
    return useQuery({
        queryKey: ['categories', organizationId],
        queryFn: async (): Promise<Category[]> => {
            const token = Cookies.get('token');
            const url = `${import.meta.env.VITE_BASE_URL}/v1/categories?organizationId=${organizationId}`;
            
            return await executeGet<Category[]>(url, token);
        },
        enabled: !!organizationId, // Only run query if organizationId is provided
    });
};

export default useCategoriesQuery;