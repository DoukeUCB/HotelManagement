import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { LOCALE_ID } from '@angular/core';                       // ✅ desde @angular/core
import { registerLocaleData } from '@angular/common';            // ✅ registerLocaleData sí es de @angular/common
import localeEsBO from '@angular/common/locales/es-BO';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';

// Registra datos del locale una sola vez
registerLocaleData(localeEsBO);

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    { provide: LOCALE_ID, useValue: 'es-BO' }                   // ✅ locale global
  ]
}).catch(console.error);
