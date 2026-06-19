import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const cookieService = inject(CookieService);
  const token = cookieService.get('auth_token');

  const authReq = req.clone({
    withCredentials: true,
    ...(token && !req.headers.has('Authorization')
      ? { setHeaders: { Authorization: `Bearer ${token}` } }
      : {})
  });

  return next(authReq);
};
