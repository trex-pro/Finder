import { CanDeactivateFn } from '@angular/router';
import { MemberProfile } from '../../features/members/member-profile/member-profile';

export const unsavedChangesGuard: CanDeactivateFn<MemberProfile> = (component: MemberProfile) => {
  if (component.editForm?.dirty) {
    return confirm('Continue? All unsaved changes will be lost.');
  }
  return true;
};
