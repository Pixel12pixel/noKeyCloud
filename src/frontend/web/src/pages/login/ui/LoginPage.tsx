import { type ActionFunctionArgs, redirect, useActionData, useNavigation } from "react-router-dom";
import { loginWithSRP } from "../api/login";
import { LoginForm } from "@/widgets/login-form/ui/LoginForm.tsx";
import { saveSession } from "@/entities/session/model/session";

export async function loginAction({ request }: ActionFunctionArgs) {
    const formData = await request.formData();
    const identifier = formData.get("identifier") as string;
    const password = formData.get("password") as string;

    if (!identifier || !password) {
        return { error: { errors: { body: ["Username or email and password are required"] } } };
    }

    try {
        const authData = await loginWithSRP(identifier, password);

        saveSession({
            userId: authData.userId,
            rootFolderId: authData.rootFolderId,
            accessTokenExpiresAt: authData.accessTokenExpiresAt
        });


        // TODO: Replace with actual folder route once implemented
        return redirect("/");
    } catch (error: any) {
        return { error: { errors: { body: [error.message || "Failed to sign in. Please verify your credentials."] } } };
    }
}

export function LoginPage() {
    const actionData = useActionData<typeof loginAction>();
    const navigation = useNavigation();
    const isSubmitting = navigation.state === "submitting";

    const errorMessage = actionData?.error?.errors?.body?.[0];

    return (
        <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
            <div className="w-full max-w-sm">
                <LoginForm error={errorMessage} isSubmitting={isSubmitting}/>
            </div>
        </div>
    );
}