import { useQuery } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executeGet } from '../../helpers/fetch';
import type { Category } from './types';

const useCategoriesQuery = () => {
    return useQuery({
        queryKey: ['categories'],
        queryFn: async (): Promise<Category[]> => {
            const token = Cookies.get('token');
            const url = `${import.meta.env.VITE_BASE_URL}/v1/categories?organizationId=F5D9445E-1C65-4CD9-8B95-44F886049FE5`;
            
            return await executeGet<Category[]>(url, token);
        }
    });
};

export default useCategoriesQuery;