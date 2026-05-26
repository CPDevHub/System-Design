import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { LucideAngularModule, ChevronLeft, ChevronRight, Sparkles, ArrowUp, Loader2, Users, Plus, Search, MoreHorizontal, ArrowLeft, AlertCircle, EyeOff, Eye, ShieldAlert, Bell, ChevronDown, LogOut } from 'lucide-angular';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    importProvidersFrom(
      LucideAngularModule.pick({ ChevronLeft, ChevronRight, Sparkles, ArrowUp, Loader2, Users, Plus, Search, MoreHorizontal, ArrowLeft, AlertCircle, EyeOff, Eye, ShieldAlert, Bell, ChevronDown, LogOut })
    )
  ]
};
