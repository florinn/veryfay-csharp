using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay.Test
{
    class PublicApiSpec : nspec
    {
        void describe_Activity()
        {
            AuthorizationEngine ae = null;

            before = () =>
            {
                ae = new AuthorizationEngine()
                    .Register(new CRUDP<Nothing>()).Allow(Admin.Instance).Deny(Supervisor.Instance, Commiter.Instance).Deny(Contributor.Instance).And
                    .Register(new CRUDP<SomeOtherClass>()).Allow(Admin.Instance).Allow(Supervisor.Instance).Allow(Reader.Instance).Allow(Contributor.Instance).And
                    .Register(new Create<Nothing>()).Allow(Commiter.Instance).Deny(Contributor.Instance).And
                    .Register(new Read<Nothing>()).Allow(Commiter.Instance).Deny(Contributor.Instance).Allow(Reviewer.Instance).And
                    .Register(new Read<SomeClass>()).Allow(Supervisor.Instance, Commiter.Instance).And
                    .Register(new Read<SomeClass>(), new Read<SomeOtherClass>()).Allow(Supervisor.Instance).Allow(Contributor.Instance).Deny(Reader.Instance).And
                    .Register(new Read<SomeClass>()).Allow(Reader.Instance).And
                    .Register(new Read<OtherSomeOtherClass>()).Allow(Reader.Instance).Deny(Commiter.Instance).Allow(Reviewer.Instance).And;
            };

            context["when action target not found"] = () =>
            {
                it["should fail"] = () =>
                {
                    var result = ae[new Create<SomeClass>()].IsAllowing(new PrincipalClass("commiter"));
                    expect<AuthorizationException>(() => ae[new Create<SomeClass>()].Verify(new PrincipalClass("commiter")));
                    result.IsFailure.should_be_true();
                };
            };
            context["when action target found"] = () =>
            {
                it["should fail when target type not matching"] = () =>
                {
                    var result = ae[new Read<OtherSomeOtherClass>()].IsAllowing(new PrincipalClass("supervisor"));
                    expect<AuthorizationException>(() => ae[new Read<OtherSomeOtherClass>()].Verify(new PrincipalClass("supervisor")));
                    result.IsFailure.should_be_true();
                };
                context["when deny role found"] = () =>
                {
                    context["once"] = () =>
                    {
                        it["should fail when principal match the deny role definition"] = () =>
                        {
                            var result = ae[new Read<Nothing>()].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Read<Nothing>()].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.should_be_true();
                        };
                        it["should succeed when principal does not match every deny role definition in a set"] = () =>
                        {
                            var result = ae[new Create<Nothing>()].IsAllowing(new PrincipalClass("commiter"));
                            ae[new Create<Nothing>()].Verify(new PrincipalClass("commiter"));
                            result.IsSuccess.should_be_true();
                        };
                        it["should fail when principal match every deny role definition in a set"] = () =>
                        {
                            var result = ae[new Create<Nothing>()].IsAllowing(new PrincipalClass("supervisor-commiter"));
                            expect<AuthorizationException>(() => ae[new Create<Nothing>()].Verify(new PrincipalClass("supervisor-commiter")));
                            result.IsFailure.should_be_true();
                        };
                        it["should fail when principal and extra info match the type of the deny role defintion"] = () =>
                        {
                            var result = ae[new Read<SomeOtherClass>()].IsAllowing(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
                            expect<AuthorizationException>(() => ae[new Read<SomeOtherClass>()].Verify(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234")));
                            result.IsFailure.should_be_true();
                        };
                        it["should succeed when principal type does not match the type of the deny role definition"] = () =>
                        {
                            var result = ae[new Read<Nothing>()].IsAllowing(new PrincipalClass("contributor"));
                            ae[new Read<Nothing>()].Verify(new PrincipalClass("contributor"));
                            result.IsSuccess.should_be_true();
                        };
                        it["should succeed when extra info type does not match the type of the deny role definition"] = () =>
                        {
                            var result = ae[new Read<OtherSomeOtherClass>()].IsAllowing(new PrincipalClass("commiter"), 1234);
                            ae[new Read<OtherSomeOtherClass>()].Verify(new PrincipalClass("commiter"), 1234);
                            result.IsSuccess.should_be_true();
                        };
                    };
                    context["more than once"] = () =>
                    {
                        it["should fail when principal and extra info match any deny role definition"] = () =>
                        {
                            var result = ae[new Read<Nothing>()].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Read<Nothing>()].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.should_be_true();
                        };
                        it["should fail when principal and extra info match any contained deny role definition"] = () =>
                        {
                            var result = ae[new Patch<Nothing>()].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Patch<Nothing>()].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.should_be_true();
                        };
                        it["should fail when principal and extra info match any deny role definition in an embedded container action"] = () =>
                        {
                            var result = ae[new Delete<Nothing>()].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Delete<Nothing>()].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.should_be_true();
                        };
                    };
                };
                context["when deny role not found"] = () =>
                {
                    context["when allow role not found"] = () =>
                    {
                        it["should fail"] = () =>
                        {
                            var result = ae[new Read<SomeClass>()].IsAllowing(new PrincipalClass("laura"));
                            expect<AuthorizationException>(() => ae[new Read<SomeClass>()].Verify(new PrincipalClass("laura")));
                            result.IsFailure.should_be_true();
                        };
                    };
                    context["when allow role found"] = () =>
                    {
                        context["once"] = () =>
                        {
                            it["should succeed when principal match the allow role definition"] = () =>
                            {
                                var result = ae[new Read<SomeOtherClass>()].IsAllowing(new OtherPrincipalClass("contributor"));
                                ae[new Read<SomeOtherClass>()].Verify(new OtherPrincipalClass("contributor"));
                                result.IsSuccess.should_be_true();
                            };
                            it["should fail when principal does not match every allow role definition in a set"] = () =>
                            {
                                var result = ae[new Read<SomeClass>()].IsAllowing(new PrincipalClass("commiter"));
                                expect<AuthorizationException>(() => ae[new Read<SomeClass>()].Verify(new PrincipalClass("commiter")));
                                result.IsFailure.should_be_true();
                            };
                            it["should succeed when principal does match every allow role definition in a set"] = () =>
                            {
                                var result = ae[new Read<SomeClass>()].IsAllowing(new PrincipalClass("supervisor-commiter"));
                                ae[new Read<SomeClass>()].Verify(new PrincipalClass("supervisor-commiter"));
                                result.IsSuccess.should_be_true();
                            };
                            it["should succeed when principal and extra info match the type of the allow role definition"] = () =>
                            {
                                var result = ae[new Read<OtherSomeOtherClass>()].IsAllowing(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
                                ae[new Read<OtherSomeOtherClass>()].Verify(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
                                result.IsSuccess.should_be_true();
                            };
                            it["should fail when principal type does not match the type of the allow role definition"] = () =>
                            {
                                var result = ae[new Read<SomeOtherClass>()].IsAllowing(new PrincipalClass("reader"), Tuple.Create(1234, "1234"));
                                expect<AuthorizationException>(() => ae[new Read<SomeOtherClass>()].Verify(new PrincipalClass("reader"), Tuple.Create(1234, "1234")));
                                result.IsFailure.should_be_true();
                            };
                            it["should fail when extra info type does not match the type of the allow role definition"] = () =>
                            {
                                var result = ae[new Read<SomeOtherClass>()].IsAllowing(new OtherPrincipalClass("reader"), 1234);
                                expect<AuthorizationException>(() => ae[new Read<SomeOtherClass>()].Verify(new OtherPrincipalClass("reader"), 1234));
                                result.IsFailure.should_be_true();
                            };
                        };
                        context["more than once"] = () =>
                        {
                            it["should succeed when principal and extra info match any allow role definition"] = () =>
                            {
                                var result = ae[new Read<SomeClass>()].IsAllowing(new PrincipalClass("supervisor"));
                                ae[new Read<SomeClass>()].Verify(new PrincipalClass("supervisor"));
                                result.IsSuccess.should_be_true();
                            };
                            it["should fail when principal and extra info do not match the param types of any allow role definition"] = () =>
                            {
                                var result = ae[new Read<SomeClass>()].IsAllowing(new OtherPrincipalClass("supervisor"));
                                expect<AuthorizationException>(() => ae[new Read<SomeClass>()].Verify(new OtherPrincipalClass("reader"), 1234));
                                result.IsFailure.should_be_true();
                            };
                            it["should succeed when principal and extra info match any contained allow role definition"] = () =>
                            {
                                var result = ae[new Patch<SomeOtherClass>()].IsAllowing(new PrincipalClass("admin"));
                                ae[new Patch<SomeOtherClass>()].Verify(new PrincipalClass("admin"));
                                result.IsSuccess.should_be_true();
                            };
                            it["should succeed when principal and extra info match any allow role definition in an embedded container action"] = () =>
                            {
                                var result = ae[new Delete<SomeOtherClass>()].IsAllowing(new PrincipalClass("admin"));
                                ae[new Delete<SomeOtherClass>()].Verify(new PrincipalClass("admin"));
                                result.IsSuccess.should_be_true();
                            };
                        };
                    };
                };
            };
        }
    }
}
