import { create } from "zustand";
import { persist, createJSONStorage } from 'zustand/middleware'
import type { User } from "../types/User";

interface UserState {
  user: User | null;
  setUser: (user: User) => void;
}

const useUserStore = create(
  persist<UserState>(
    (set) => ({
      user: null,
      setUser: (user: User) => {
        set(() => ({
          user
        }));
      }
    }),
    {
      name: 'user-storage',
      storage: createJSONStorage(() => localStorage),
    }
  )
);

export default useUserStore;